using Microsoft.EntityFrameworkCore;
using SpendSense.Data;
using SpendSense.Models;

namespace SpendSense.Services
{
    public class AccountService
    {
        private readonly AppDbContext _context;

        public AccountService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Account>> GetAllByUserId(int userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Account?> GetById(int id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task Add(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

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