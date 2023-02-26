using Microsoft.AspNetCore.Mvc;
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

        public AuthController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public static User user = new User();
        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserDTO usr)
        {
            CreatePasswordHash(usr.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

        }
        private void CreatePasswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt) 
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        
    }
}
