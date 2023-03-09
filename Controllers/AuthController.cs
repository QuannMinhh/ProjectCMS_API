﻿using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public static User user = new();
        private readonly IConfiguration _configuration;
        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        [HttpGet("Profile"),AllowAnonymous]
        public async Task<IActionResult> EndPoint()
        {
            var currentUser = await _dbContext._users.FirstOrDefaultAsync(uId => uId.UserId == GettUserId());
            User usr = new User
            {
                UserId = currentUser.UserId,
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Phone = currentUser.Phone,
                DoB = currentUser.DoB,
                Address = currentUser.Address,
                Avatar = currentUser.Avatar,
                AddedDate = currentUser.AddedDate,
                Role = currentUser.Role,
            };
            return Ok(usr);
        }        
        [HttpGet,AllowAnonymous]
        public async Task<IActionResult> getAllUser()
        {
            List<User> users = await _dbContext._users.ToListAsync();
            return Ok(users);
        }
        [HttpPost("Register"),Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateAccount(UserDTO usr)
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
        [HttpPost("Login"),AllowAnonymous]
        public async Task<IActionResult> Login(UserLogin rq)
        {
            /*List<User> users = await _dbContext._users.ToListAsync();
            
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
            */
             
            var currentUser = await _dbContext._users.FirstOrDefaultAsync(u => u.UserName == rq.userName);
            if(currentUser != null)
            {
                if (Verify(rq.password, currentUser.PasswordHash, currentUser.PasswordSalt))
                {
                    string token = tokenMethod(currentUser);
                    return Ok(token);
                }    
            }
            return BadRequest("Wrong username or password");
        }
        public async Task<IActionResult> ForgotPassword(string userName) 
        {

            string newPassword = RandomString(8);
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            return Ok("Check your email!");
        }

        private string tokenMethod(User user)
        {
            List<Claim> claims = new()
            {
                
                new Claim(ClaimTypes.SerialNumber,user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.UserName),
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
        
        public int GettUserId()
        {           
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity !=null)
            {
                var userClaim = identity.Claims;               
                return Int32.Parse(userClaim.FirstOrDefault(o => o.Type == ClaimTypes.SerialNumber)?.Value);                
                
            }            
            return 0;
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
