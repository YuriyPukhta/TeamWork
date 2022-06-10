using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using QuiQue.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using QuiQue.Models.View;
using Microsoft.EntityFrameworkCore;
using QuiQue.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace QuiQue.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/queues/{queueId}/system")]
    public class SystemUserController : Controller
    {
        private readonly int salt = 12;

        private readonly ILogger<SystemUserController> _logger;

        private QuickQueueContext _context;

        private readonly IJWTAuthenticationManager _JWTAuthenticationManager;

        private readonly IHubContext<QueueHub> _queueHub;

        public SystemUserController(QuickQueueContext context, ILogger<SystemUserController> logger, IJWTAuthenticationManager jWTAuthenticationManager, IHubContext<QueueHub> queueHub)
        {
            _context = context;
            _logger = logger;
            _JWTAuthenticationManager = jWTAuthenticationManager;
            _queueHub = queueHub;
        }


        [Route("/get_queue/{queueId}")]
        [HttpGet]
        public async Task<IActionResult> QueueGetUpdate([FromRoute] int queueId)
        {
            List<Queue> queue = await _context.Queues.Where(qid => qid.EventId == queueId && qid.Status == "in queue").OrderBy(e =>e.Number).ToListAsync();

            if (queue.Count() == 0)
                return new OkObjectResult(new List<Queue>());
            // convert to view 

            return new OkObjectResult(queue);
        }

        [Route("/get_queue_array/{queueId}")]
        [HttpGet]
        public async Task<IActionResult> QueueArrayGetUpdate([FromRoute] int queueId)
        {
            List<String> queue = await _context.Queues.Where(qid => qid.EventId == queueId && qid.Status == "in queue").OrderBy(e => e.Number).Select(e => e.Username).ToListAsync();

            if (queue.Count() == 0)
                return new OkObjectResult(new List<String>());
            // convert to view 
            return new OkObjectResult(queue);
        }

        [Route("/user/change")]
        [HttpPut]
        public async Task<IActionResult> UserChange([FromBody] UserModel user)
        {
            Int64 OwnerId = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user_before = await _context.Users.FirstOrDefaultAsync(c => c.idUser == OwnerId);
            if (user_before == null)
                return Forbid();
            if (user.Username == null || user.Username.Length < 3 || user.Username.Length > 16)
            {
                return BadRequest("Too short or too long title");
            }
            /*
            if (user.Email == null || !user.Email.Contains("@") || !user.Email.Contains(".") || user.Email.Length < 7)
            {
                return BadRequest("Too short or too long title");
            }
            //повтор емейлу
            if (user_before.Email != user.Email)
            {
                User user_after = await _context.Users.FirstOrDefaultAsync(c => c.idUser == OwnerId);
                if (user_after != null)
                {
                    return BadRequest("Wrong email");
                }
            }
            */
            //user_before.Email = user.Email;
            user_before.Username = user.Username;

            if (user.PhoneNumber != null)
                user_before.PhoneNumber = user.PhoneNumber;
            //user_before.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);

            _context.Update(user_before);
            await _context.SaveChangesAsync();

            return new OkObjectResult(user_before);
        }


        [Route("/queue/enter/{EventId}")]
        [HttpPost]
        public async Task<IActionResult> EnterQueue([FromRoute] Int64 EventId)
        {

            Int64 idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user = await _context.Users.FirstOrDefaultAsync(u => u.idUser == idUser);

            Event eve = await _context.Events.FirstOrDefaultAsync(e => e.EventId == EventId);
            if (eve == null) //чи існує івент
                return NotFound("!!!");

            if (eve.OwnerId == idUser) // хоче записатися у свою чергу
                return Forbid("you are admin. think about user don't take their place");

            if (await _context.Queues.FirstOrDefaultAsync(u => u.idUser == idUser && u.EventId == EventId && u.Status != "pass") is not null) //повторний запис?
                return UnprocessableEntity("you are arlady in queue");

            if (eve.IsSuspended)
                return UnprocessableEntity("Event is suspended!");

            //формування нового запису в чергу 
            Queue new_position = new Queue();
            new_position.Username = user.Username;
            new_position.idUser = idUser;
            new_position.EventId = EventId;

            List<Queue> queues = await _context.Queues.Where(e => e.EventId == EventId).ToListAsync();
            if (queues.Count == 0)
                new_position.Number = 1;
            else
                new_position.Number = queues.Max(e => e.Number) + 1;

            //List<Queue> queues = await _context.Queues.Where(e => e.EventId == EventId).ToListAsync();
            // new_position.Number = queues1 == null ? 1 : queues1.Number + 1;
            //queues1
            //List <Queue> queues = await _context.Queues.Where(e => e.EventId == EventId).ToListAsync();
            //new_position.Number = queues.LastOrDefault() == null ? 1 : queues.Last().Number + 1;

            new_position.Time_queue = DateTime.UtcNow;
            new_position.Status = "in queue";
            //new_position.User = _context.Users.FirstOrDefault(u => u.idUser == idUser);

            // може допоможе з поясвою  однакових номерів
            /*if (await _context.Queues.FirstOrDefaultAsync(e => e.Number == queues1.Number && e.Status != "pass") != null)
                return BadRequest();*/

            _context.Add(new_position);
            _context.SaveChanges();
            await _queueHub.Clients.Group(EventId.ToString()).SendAsync("Sendqueue", "this queue id " + EventId + " was changed");
            return new OkObjectResult(new_position);
        }


        [Route("/queue/delete/{EventId}")]
        [HttpDelete]
        public async Task<IActionResult> LeaveQueue([FromRoute] Int64 EventId)
        {
            Int64 idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Queue deleted_queue = await _context.Queues.FirstOrDefaultAsync(q => q.EventId == EventId && q.idUser == idUser && q.Status != "pass");
            if (deleted_queue is null)
            {
                return NotFound();
            }


            _context.Remove(deleted_queue);
            _context.SaveChanges();
            await _queueHub.Clients.Group(EventId.ToString()).SendAsync("Sendqueue", "this queue id " + EventId + " was changed");
            return new OkResult();
        }


        [Route("/get_my_event")]
        [HttpGet]
        public async Task<IActionResult> MyEvent()
        {
            Int64 idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);


            List<Event> Event = await _context.Events.Where(e => e.OwnerId == idUser).ToListAsync();
            //List<EventModel> eventModels = new List<EventModel>();

            if (Event.Count() == 0)
            {
                return NotFound("No info");
            }
            return new OkObjectResult(Event);
        }


        [Route("/get_not_my_event")]
        [HttpGet]
        public async Task<IActionResult> NotMyEvent()
        {
            Int64 idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);


            List<Queue> Queue = await _context.Queues.Where(e => e.idUser == idUser).ToListAsync();
            List<Event> Event = new List<Event>();
            for (int i = 0; i < Queue.Count; i++)
            {
                Event.Add(_context.Events.FirstOrDefault(e => e.EventId == Queue[i].EventId));
            }

            if (Event.Count() == 0)
            {
                return NotFound("No info");
            }
            return new OkObjectResult(Event);
        }

        private List<Event> fix(List<Event> events)
        {
            List<Event> eventsresult = new List<Event> {};
            long Id = -1;

            foreach (var E in events)
            {
                if(E.EventId != Id)
                {
                    eventsresult.Add(E);
                }
                Id = E.EventId;
            }

            return eventsresult;
        }
        

        [Route("/get_my_queue")]
        [HttpGet]
        public async Task<IActionResult> MyQeueu()
        {
            Int64 idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (await _context.Users.FirstOrDefaultAsync(u => u.idUser == idUser) is null)
                return BadRequest();

            var events =  await _context.Events.Join(_context.Queues,
                e => e.EventId,
                q => q.EventId,
                
                (e, q) => new { e, q }).Where(z => z.q.idUser == idUser)
                .Select(z => new Event
                {
                    EventId = z.e.EventId,
                    OwnerId = z.e.OwnerId,
                    Title = z.e.Title,
                    Description = z.e.Description
                })
                .OrderBy(z=> z.EventId).ToListAsync();  //

            return new OkObjectResult(fix(events));//
        }
    }
}

