using Microsoft.EntityFrameworkCore;
using SpendSense.Data;
using SpendSense.Models;
using System.Security.Cryptography;
using System.Text;

namespace SpendSense.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Register(User user)
        {
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);

            if (emailExists)
                return false;

            user.Password = HashPassword(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User?> Login(string email, string password)
        {
            string hashedPassword = HashPassword(password);

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedPassword);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();

            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(bytes);
        }
    }
}