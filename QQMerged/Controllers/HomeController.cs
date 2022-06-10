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
using System.Threading;

namespace QuiQue.Controllers

{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        private QuickQueueContext _context;

        private readonly IJWTAuthenticationManager _JWTAuthenticationManager;

        public HomeController(QuickQueueContext context, ILogger<HomeController> logger, IJWTAuthenticationManager jWTAuthenticationManager)
        {
            _context = context;
            _logger = logger;
            _JWTAuthenticationManager = jWTAuthenticationManager;
        }

        [Route("/my_account")]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            Int64 idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            User user = await _context.Users.FirstOrDefaultAsync(e => e.idUser == idUser);
            user.Password = "";

            return new OkObjectResult(user);
        }

        // отримати інформацію про івент за ID
        [Route("/event/{idEvent}")]
        [HttpGet]
        public async Task<IActionResult> GetEvent([FromRoute] Int64 idEvent)
        {
            Event Event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == idEvent);
            if (Event == null)
            {
                return NotFound();
            }
            return new OkObjectResult(Event);
        }
    }
}