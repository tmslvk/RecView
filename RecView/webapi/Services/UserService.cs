using Microsoft.EntityFrameworkCore;
using webapi.DTO;
using webapi.Models;

namespace webapi.Services
{
    public class UserService
    {
        ApplicationContext db;

        public UserService(ApplicationContext context)
        {
            this.db = context;
        }

        public async Task<User> Add(UserRegDTO userDTO)
        {
            var user = new User()
            {
                Lastname = userDTO.Lastname,
                Firstname = userDTO.Firstname,
                Email = userDTO.Email,
                Password = userDTO.Password,
                Country = userDTO.Country,
                Username = userDTO.Username,
            };
            await db.AddAsync(user);
            await db.SaveChangesAsync();

            return user;
        }

        public async Task<User?> Login(UserLogDTO loginDto)
        {
            User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email || u.Username == loginDto.Username);
            if (user == null)
            {
                return null;
            }
            bool verified = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);
            if (!verified)
            {
                return null;
            }
            db.Entry(user);
            return user;
        }

        public async Task<User?> GetOne(int id)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
                db.Entry(user);
            return user;
        }

        public async Task<bool> CheckUsername(string username)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user != null;
        }
    }
}
