using Courrier.DAL;
using Courrier.Models;
using System.Data.Entity;

namespace Courrier.Services
{
    public class UserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext context)
        {
            _dbContext = context;
        }

        public User findByEmail(string email)
        {
            return _dbContext.Users.Include(user=> user.Role)
                    .FirstOrDefault(u => u.Email == email);
        }

        public List<User> GetAllCoursier()
        {
            return _dbContext.Users.Where(u => u.IdRole == 4).ToList();
        }
    }
}
