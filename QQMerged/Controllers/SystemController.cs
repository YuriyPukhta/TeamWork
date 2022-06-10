using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuiQue.Models;
using Microsoft.EntityFrameworkCore;
using QuiQue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using QuiQue.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace QuiQue.Controllers
{
    public class SystemController : Controller
    {
        private readonly ILogger<SystemController> _logger;

        private QuickQueueContext _context;

        private readonly IJWTAuthenticationManager _JWTAuthenticationManager;

        private readonly IHubContext<QueueHub> _queueHub;
        public SystemController(QuickQueueContext context, ILogger<SystemController> logger, IJWTAuthenticationManager jWTAuthenticationManager, IHubContext<QueueHub> queueHub)
        {
            _context = context;
            _logger = logger;
            _JWTAuthenticationManager = jWTAuthenticationManager;
            _queueHub = queueHub;
        }


        [Authorize]
        [Route("/queue/create")]
        [HttpPost]
        public IActionResult PostCreateEvent([FromBody] Event Event)
        {
            // Без Title
            if (Event.Title == null) return BadRequest();
            // перевірити чи всі поля правильні
            if (Event.Title.Length > 50 || Event.Title.Length < 3)
            {
                return BadRequest("Too short or too long title");
            }

            Event.OwnerId = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            _context.Add(Event);
            _context.SaveChanges();
            return new OkObjectResult(Event);
        }


        [Authorize]
        [Route("/queue/{idEvent}/moder/delete")]
        [HttpDelete]
        public async Task <IActionResult> QueueIdModerSystemDelete([FromRoute] Int64 idEvent, [FromQuery] Int64 idUser)
        {
            Int64 OwnerId = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            User user = await _context.Users.FirstOrDefaultAsync(c => c.idUser == idUser);
            if (user == null)
            {
                return BadRequest();
            }

            Queue queue = await _context.Queues.FirstOrDefaultAsync(c => c.idUser == idUser && c.EventId == idEvent);
            Event evnt = await _context.Events.FirstOrDefaultAsync(c => c.EventId == idEvent);
            // Чи в токені лежить id модератора
            if (evnt == null || evnt.OwnerId != OwnerId) return UnprocessableEntity();

            // Перевіряю чи є така черга взагалі для того, щоб видалити
            try
            {
                _context.Remove(queue);
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest();
            }
            await _queueHub.Clients.Group(idEvent.ToString()).SendAsync("Sendqueue", "this queue id " + idEvent + " was changed");
            return new OkObjectResult(queue);
        }


        [Authorize]
        [Route("/system/get_my_id")]
        [HttpGet]
        public async Task<IActionResult> Auth()
        {
            Int64 Userid = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            User ISuser = await _context.Users.FirstOrDefaultAsync(c => c.idUser == Userid);
            // Чи в токені лежить id модератора
            if (ISuser == null)
                return NotFound();
            return Ok(Userid);
        }


        [Authorize]
        [Route("/queue/{idEvent}/moder/update")]
        [HttpPut]
        public async Task<IActionResult> QueueIdModerSystemUpdate([FromRoute] Int64 idEvent, [FromBody] Event ev)
        {
            Int64 OwnerId = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Event evnt = await _context.Events.FirstOrDefaultAsync(c => c.EventId == idEvent);
            // перевіряю наявність потрібних даних
            if (evnt == null || ev.Title.Length < 4 || ev.Title.Length > 50)
            {
                return BadRequest();
            }
            evnt.Title = ev.Title;

            if (evnt.OwnerId != OwnerId)
            {
                return Forbid();
            }
            try
            {
                _context.Update(evnt);
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest();
            }
            await _queueHub.Clients.Group(idEvent.ToString()).SendAsync("Sendqueue", "this queue id " + idEvent + " was changed");
            return new OkObjectResult(evnt);
        }
        // юра
        private async Task<Queue> nextuser(Int64 eventid, Event eve)
        {
            if (eve.IsSuspended)
                return null;
            Queue change =  await _context.Queues.Where(o => o.Status != "pass" && o.EventId == eventid).OrderBy(o => o.Number).FirstOrDefaultAsync();
            // чи є користувачі в черзі для пропуску
            if (change == null)
                return null;//"nobody is waiting on queue";
            change.Status = "pass";

            await _context.SaveChangesAsync();
            return change;//"Ok";
        }
        private async Task<bool> close(Event Event)
        {
            // чи вже закрита 
            if (Event.IsSuspended)
                return false;

            Event.IsSuspended = true;
            //_context.Update(Event);
           await _context.SaveChangesAsync();
            return true;
        }
        private bool finish(Event Event)
        {
            // поідеї тут каскадне видалення 
            _context.Remove(Event);
            _context.SaveChanges();
            return true;
        }

        private async Task<bool> open(Event Event)
        {
            // чи вже відкрита 
            if (!Event.IsSuspended)
                return false;

            Event.IsSuspended = false;
            //_context.Update(Event);
            await _context.SaveChangesAsync();
            return true;
        }


        [Authorize]
        [Route("/queue/{idEvent}/moder/{value}/")]
        [HttpPut]
        public async Task<IActionResult> QueueIdModerSystemNext([FromRoute] Int64 idEvent, [FromRoute] string value)
        {
            Int64 OwnerId = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Event evnt = await _context.Events.FirstOrDefaultAsync(c => c.EventId == idEvent);
            // існує такий івент 
            if (evnt == null)
            {
                return NotFound();
            }
            // чи зміни вносить власник черги 
            if (OwnerId != evnt.OwnerId)
                return UnprocessableEntity();

            switch (value)
            {
                case "next":
                    var resalt = await nextuser(idEvent, evnt);
                    if (resalt == null)
                        return BadRequest();
                    else
                        return new OkObjectResult(resalt);
                case "close":
                    if (await close(evnt))
                        return new OkResult();
                    else
                        return BadRequest();
                case "finish":
                    if (finish(evnt))
                        return new OkResult();
                    else
                        return BadRequest(":(((");
                case "open":
                    if (await open(evnt))
                        return new OkResult();
                    else
                        return BadRequest(":(((");
                default:
                    return BadRequest("Default Error");
            }
        }

        //////////////////////////////////////
        [Authorize]
        [Route("/get_my_id")]
        [HttpGet]
        public IActionResult IS()
        {
            Int64 Userid = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            User ISuser = _context.Users.FirstOrDefault(c => c.idUser == Userid);
            if (ISuser == null)
                return NotFound();
            return Ok(Userid);
        }


        [Authorize]
        [Route("/IOwner/{idEvent}")]
        [HttpGet]
        public async Task<IActionResult> IOwner([FromRoute] Int64 idEvent)
        {
            Int64 Userid = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Event Event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == idEvent);
            if (Event == null)
                return NotFound();
            if (Event.OwnerId == Userid)
                return Ok(true);
            else
                return Ok(false);
        }
    }
}
