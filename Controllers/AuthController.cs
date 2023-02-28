using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ProjectCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public User user = new();
        private readonly IConfiguration _configuration;
        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> getAllUser()
        {
            List<User> users = await _dbContext._users.ToListAsync();
            return Ok(users);
        }
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO usr)
        {
            CreatePasswordHash(usr.password, out byte[] passwordHash, out byte[] passwordSalt);
            
            user.UserName= usr.userName;
            user.Email = usr.Email;
            user.Role = usr.Role;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;       
            _dbContext._users.Add(user);
            _dbContext.SaveChanges();
            return Ok(user);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDTO rq)
        {
            List<User> users = await _dbContext._users.ToListAsync();
            
            foreach (var user in users) 
            {
                if(user.UserName == rq.userName) 
                {                   
                    if (Verify(rq.password, user.PasswordHash, user.PasswordSalt))
                    {
                        string token = tokenMethod(user);
                        return Ok(token);
                    }                   
                }
            }
            return BadRequest("wrong username or password");
        }
        private string tokenMethod(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name,user.UserName),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken
                (
                    claims:claims,
                    expires:DateTime.Now.AddDays(1),
                    signingCredentials:cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        private void CreatePasswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt) 
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool Verify(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        
    }
}
