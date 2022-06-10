using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using QuiQue.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using QuiQue.Service;
using JWT.Algorithms;
using JWT.Serializers;
using JWT;
using JWT.Builder;

namespace QuiQue.Controllers
{
    [ApiController]
    public class TokenMaster : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        private QuickQueueContext _context;

        private readonly IJWTAuthenticationManager _JWTAuthenticationManager;

        private readonly ITokenManager _tokenManager;

        IEmailSender _emailSender;
        public TokenMaster(IEmailSender emailSender, QuickQueueContext context, ILogger<AuthController> logger, IJWTAuthenticationManager jWTAuthenticationManager, ITokenManager tokenManager)
        {
            _context = context;
            _logger = logger;
            _JWTAuthenticationManager = jWTAuthenticationManager;
            _tokenManager = tokenManager;
            _emailSender = emailSender;
        }
        const string secret = "GQDstcKsxfgdghhgkdajldkgufpit2=-3593548tgierwuewtuy98wyw934675498yg98460NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        [AllowAnonymous]
        [HttpPost]
        [Route("/token/create/{model}")]
        public ActionResult create(string  model)
        {
            ActionResult response = null;
            string token;

                if (true) //todo: check user login&password
                {
                token = JwtBuilder.Create()
                  .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                  .WithSecret(secret)
                  .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
                  .AddClaim("email", "model")
                  .Encode();
                }
                else
                {
                response = BadRequest();
                }
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/token/decoding/{token}")]
        public ActionResult decoding(string token)
        {
            var json = JwtBuilder.Create()
                      .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                      .WithSecret(secret)
                      .MustVerifySignature()
                      .Decode(token);
            return Ok(json);
        }

    }
}
