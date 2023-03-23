using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.Services;
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
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public static User user = new();
        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration, EmailService emailservice, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailservice;
            _env = env;
        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstyvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        

        
        [HttpGet("Profile")]
        public async Task<IActionResult> EndPoint()
        {
            var currentUser = await _dbContext._users
                .Join(_dbContext._departments, _usr => _usr.DepartmentID, _dep =>_dep.DepId, (_usr, _dep) => new UserViewModel
                {
                    UserId = _usr.UserId,
                    UserName = _usr.UserName,
                    Email = _usr.Email,
                    Phone = _usr.Phone,
                    DoB = _usr.DoB,
                    Address = _usr.Address,
                    Avatar = _usr.Avatar,
                    AddedDate = _usr.AddedDate,
                    Role = _usr.Role,
                    Department = _dep.Name,
                })
                .FirstOrDefaultAsync(uId => uId.UserId == GettUserId());
            return Ok(currentUser);
        }        
        [HttpGet]
        public async Task<IActionResult> getAllUser()
        {
            var users = await _dbContext._users
                .Join(_dbContext._departments, _usr => _usr.DepartmentID, _dep => _dep.DepId, (_usr, _dep) => new UserViewModel
                {
                    UserId = _usr.UserId,
                    UserName = _usr.UserName,
                    Email = _usr.Email,
                    Phone = _usr.Phone,
                    DoB = _usr.DoB,
                    Address = _usr.Address,
                    Avatar = _usr.Avatar,
                    AddedDate = _usr.AddedDate,
                    Role = _usr.Role,
                    Status = _usr.Status,
                    Department = _dep.Name,
                }).ToListAsync();
            return Ok(users);
        }
        [HttpPut("UpdateAvatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] UpdateAvatar avatar)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.Claims;
                var username = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value;
                var user = _dbContext._users.FirstOrDefault(user => user.UserName == username);
                {
                    if (user != null)
                    {
                        if (avatar.Image.Length > 0)
                        {
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", avatar.Image.FileName);
                            using (var stream = System.IO.File.Create(path))
                            {
                                await avatar.Image.CopyToAsync(stream);
                            }
                            user.Avatar = "/images/" + avatar.Image.FileName;
                            await _dbContext.SaveChangesAsync();
                            return Ok("Update success");
                        }
                    }
                }
            }
                return BadRequest("Upload avatar failed");
        }
        [HttpGet(),AllowAnonymous]
        [Route("Download/{directoryName}")]
        public async Task<IActionResult> DownloadFile([FromRoute]string directoryName)
        {
            var (fileType,bytes,fileName) = new FileService(_env).DownloadZip(directoryName);
            return File(bytes,fileType,fileName);
        }
        [HttpGet("DownloadCSV"),AllowAnonymous]
        public async Task<IActionResult> DownloadCSV()
        {
            int i = new FileService(_env).GPT();
            return Ok(i);
        }
        [HttpPost("CreateAccount"),Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateAccount(UserDTO usr)
        {
            CreatePasswordHash(usr.password, out byte[] passwordHash, out byte[] passwordSalt);
            User user = new User();

                user.UserName = usr.UserName;
                user.Email = usr.Email;
                user.Role = usr.Role;
                user.DepartmentID = usr.DepartmentID;
                user.Avatar = "/images/Avatar.jpg";
                if (usr.Phone != null)
                {
                    user.Phone = usr.Phone;
                }
                else user.Phone = "1900 1001";

                user.AddedDate = usr.AddedDate;
                if (usr.Address != null)
                {
                    user.Address = usr.Address;
                }
                else user.Address = "Default address";
                if (usr.DoB != null)
                {
                user.DoB = usr.DoB;
                }
                else user.DoB = DateTime.Parse("2000-01-01");
                user.Status = "Enable";
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            
            _dbContext._users.Add(user);
            _dbContext.SaveChanges();
            return Ok(user);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> EditAccount([FromForm] UserUpdate usr)
        {
            
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if(identity !=  null)
            {
                var userClaim = identity.Claims;
                var username = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value;
                var User = await _dbContext._users.FirstOrDefaultAsync(u => u.UserName == username);
                if(User != null)
                {
                    if(Verify(usr.Password,User.PasswordHash,User.PasswordSalt))
                    {
                        //Bí Thuật
                        if (usr.Address != null)
                        {
                            User.Address = usr.Address;
                        }
                        else { User.Address = User.Address; }
                        if(usr.Avatar != null)
                        {
                            User.Avatar = usr.Avatar;
                        }    
                        else { User.Avatar = User.Avatar; }
                        if(usr.Email != null)
                        {
                            User.Email = usr.Email;
                        }
                        else { User.Email = User.Email; }
                        if(usr.Phone != null) 
                        {
                            User.Phone = usr.Phone;
                        }
                        else { User.Phone = User.Phone; }
                        if(usr.DoB != null)
                        {
                            User.DoB = usr.DoB;
                        }    
                        else { User.DoB = User.DoB; }
                        //Internal 500
                        
                            if (usr.Image.Length > 0)
                            {
                                //Add userName
                                string ext = Path.GetExtension(usr.Image.FileName);
                            DateTime dt = DateTime.Now;
                            string fileName = User.UserName + ext;
                                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                                using (var stream = System.IO.File.Create(path))
                                {
                                    await usr.Image.CopyToAsync(stream);
                                }
                                User.Avatar = "/images/" + fileName;

                            }
                        
                        await _dbContext.SaveChangesAsync();
                        return Ok(User);    
                    }
                }    
            }
            return BadRequest();
        }
        [HttpPut]
        [Route("{usr}")]
        public async Task<IActionResult> DeleteAccount([FromForm]string password, [FromRoute]string usr)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.Claims;
                var username = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value;
                var User = await _dbContext._users.FirstOrDefaultAsync(u => u.UserName == username);
                if(User !=null)
                {
                    if(Verify(password, User.PasswordHash, User.PasswordSalt)&& User.Role=="Admin")
                    {
                        var user = await _dbContext._users.FirstOrDefaultAsync(u => u.UserName == usr);
                        if(user != null)
                        {
                            user.Status = "Disable";
                            await _dbContext.SaveChangesAsync();
                            return Ok();
                        }
                    }
                }
            }                     
            return BadRequest();
        }
        [HttpPost("Login"),AllowAnonymous]
        public async Task<IActionResult> Login(UserLogin rq)
        {
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
        [HttpPost("Resetpassword"),AllowAnonymous]       
        public async Task<IActionResult> ResetPassword(FPass pwd) 
        {
            var user = await _dbContext._users.FirstOrDefaultAsync(usrname => usrname.UserName == pwd.userName && usrname.Email == pwd.email);
            if(user != null)
            {
                string newPassword = RandomString(8);
                CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                _emailService.ForgotPassword(newPassword,user.Email);
                await _dbContext.SaveChangesAsync();
                return Ok("Please Check your Email");
            }    

            return BadRequest("Wrong username or Email");
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
