using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using QuiQue.Models;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace QuiQue
{
    public interface IJWTAuthenticationManager
    {
        public Task<string> Authenticate(string email, string password);
        public Task<bool> Registration(User new_user);
        public Task<bool> Registrationconfirm(User new_user);
    }
    //генерація токенів, створення користувачів
    public class JWTAuthenticationManager : IJWTAuthenticationManager
    {

        private readonly string tokenKey;
        private readonly QuickQueueContext _context;
        private readonly int salt = 12;

        public JWTAuthenticationManager(string tokenKey, QuickQueueContext context)
        {
            this.tokenKey = tokenKey;
            this._context = context;
        }

        public async Task<String> Authenticate(string email, string password)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
            {
                return "no user email";
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return "bad password";
            }
            if (!user.Confirm)
            {
                return "confirm your email";
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tokenKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.NameIdentifier, user.idUser.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(28),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<bool> Registration(User new_user)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == new_user.Email);
            //перевірка чи існує такий користувач
            if (user is not null)
            {
                return false;
            }
            //наскільки нормальний в нього юзернейм, пробач Ян
            if (new_user.Username.Length < 3 || new_user.Username.Length > 16)
            {
                return false;
            }
            //придумали нормальний пароль?
            else if (new_user.Password.Length < 8 || new_user.Password.Length > 20)
            {
                return false;
            }
            //ввели пошту чи простий рядок? хоча "@" нічого ще не означає
            if (!new_user.Email.Contains("@") || !new_user.Email.Contains(".") || new_user.Email.Length < 7)
            {
                return false;
            }
            /*
            string email = new_user.Email;
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (!re.IsMatch(email))*/
            new_user.Confirm = true;
            new_user.Password = BCrypt.Net.BCrypt.HashPassword(new_user.Password, salt);
                await _context.AddAsync(new_user);
                await _context.SaveChangesAsync();
                return true;
        }
        public async Task<bool> Registrationconfirm(User new_user)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == new_user.Email);
            //перевірка чи існує такий користувач
            if (user is not null)
            {
                return false;
            }
            //наскільки нормальний в нього юзернейм, пробач Ян
            if (new_user.Username.Length < 3 || new_user.Username.Length > 16)
            {
                return false;
            }
            //придумали нормальний пароль?
            else if (new_user.Password.Length < 8 || new_user.Password.Length > 20)
            {
                return false;
            }
            //ввели пошту чи простий рядок? хоча "@" нічого ще не означає
            if (!new_user.Email.Contains("@") && !new_user.Email.Contains(".") && new_user.Email.Length < 7)
            {
                return false;
            }
            /*
            string email = new_user.Email;
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (!re.IsMatch(email))*/
            new_user.Confirm = false;
            new_user.Password = BCrypt.Net.BCrypt.HashPassword(new_user.Password, salt);
            await _context.AddAsync(new_user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
    public class UserCredentials //просто зручний клас для передавання даних користувача, хай буде
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}