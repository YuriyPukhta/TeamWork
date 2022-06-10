using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JWT.Builder;
using JWT.Algorithms;

namespace QuiQue
{
    public class Tocken_Master
    {
        private const string secret = "GQDstcKsxfgdghhgkdajldkgufpit2=-3593548tgierwuewtuy98wyw934675498yg98460NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        public Tocken_Master()
        {
        }

        public string CreateToken (string Email)
        {
            string token;

            token = JwtBuilder.Create()
                  .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                  .WithSecret(secret)
                  .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
                  .AddClaim("email", Email)
                  .Encode();

            return token;
        }
        public string DecodToken (string token)
        {
            var json = JwtBuilder.Create()
                      .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                      .WithSecret(secret)
                      .MustVerifySignature()
                      .Decode(token);
            return json;
        }
    }
}
