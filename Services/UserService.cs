using ProjectCMS.Data;
using ProjectCMS.Models;

namespace ProjectCMS.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;
        public User user = new();
        private readonly IConfiguration _configuration;
        public UserService(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
    }
}
