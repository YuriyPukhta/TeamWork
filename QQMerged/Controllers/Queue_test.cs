using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using QuiQue.Service;

namespace QuiQue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetQueueTestController : ControllerBase
    {
        private readonly ILogger<GetQueueTestController> _logger;

        IEmailSender _emailSender;
        public GetQueueTestController(ILogger<GetQueueTestController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IEnumerable<QueueTest> Get()
        {
            return Enumerable.Range(1, 30).Select(index => new QueueTest
            {
                name = "User Name " + index.ToString()
            })
            .ToArray();
        }

        [HttpGet(nameof(Get1))]
        public int Get1()
        {
            Response.Redirect("/");
            return 1;
        }

    }
}
