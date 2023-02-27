using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using System.Security.Cryptography;

namespace ProjectCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public User user = new();

        public AuthController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
            CreatePasswordHash(usr.Password, out byte[] passwordHash, out byte[] passwordSalt);
            
            user.UserName= usr.UserName;
            user.Email = usr.Email;
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
            foreach (User user in users) 
            {
                if(user.UserName != rq.UserName) 
                {
                    return BadRequest("Wrong username ");
                    
                }
                if (!Verify(rq.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("wrong password");
                }
            }
            return Ok("Login success");
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
