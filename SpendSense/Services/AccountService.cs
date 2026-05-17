using Microsoft.EntityFrameworkCore;
using SpendSense.Data;
using SpendSense.Models;

namespace SpendSense.Services
{
    public class AccountService
    {
        private readonly AppDbContext _context;

        // Injects the AppDbContext dependency via constructor
        public AccountService(AppDbContext context)
        {
            _context = context;
        }

        // Returns all accounts that belong to the specified user
        public async Task<List<Account>> GetAllByUserId(int userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        // Finds and returns a single account by its primary key, or null if not found
        public async Task<Account?> GetById(int id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        // Adds a new account to the database and persists the change
        public async Task Add(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        // Updates an existing account record in the database and persists the change
        public async Task Update(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        // Removes an account by ID from the database if it exists
        public async Task Delete(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }
    }
}
