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
    [Route("api/auth")]
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
            User user = new User
            {
                UserName = usr.userName,
                Email = usr.Email,
                Role = usr.Role,
                Phone = usr.Phone,
                Address = usr.Address,
                DoB = usr.DoB,
                AddedDate = usr.AddedDate,
                Avatar = usr.Avatar,
                DepartmentID = usr.DepartmentID,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _dbContext._users.Add(user);
            _dbContext.SaveChanges();
            return Ok(user);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLogin rq)
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
            return BadRequest(new { message = "wrong username or password" });
        }
        private string tokenMethod(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())
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
