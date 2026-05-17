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

        // Injects the AppDbContext dependency via constructor
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Registers a new user: returns false if the email is already taken,
        // otherwise hashes the password and persists the user
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

        // Looks up a user by email and hashed password; returns null if credentials do not match
        public async Task<User?> Login(string email, string password)
        {
            string hashedPassword = HashPassword(password);

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedPassword);
        }

        // Computes a SHA-256 hash of the given plain-text password and returns it as a Base64 string
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();

            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(bytes);
        }
    }
}
