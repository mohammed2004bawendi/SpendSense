using Microsoft.EntityFrameworkCore;
using SpendSense.Data;
using SpendSense.Models;

namespace SpendSense.Services
{
    public class TransactionService
    {
        private readonly AppDbContext _context;

        // Injects the AppDbContext dependency via constructor
        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        // Returns all transactions for the specified user, ordered by date descending, with linked account data
        public async Task<List<Transaction>> GetAllByUserId(int userId)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        // Returns a single transaction by ID with its linked account, or null if not found
        public async Task<Transaction?> GetById(int id)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // Saves a new transaction and adjusts the linked account's balance
        // (increments for Income, decrements for Expense)
        public async Task Add(Transaction transaction)
        {
            var account = await _context.Accounts.FindAsync(transaction.AccountId);

            if (account != null)
            {
                if (transaction.TransactionType == "Income")
                {
                    account.Balance += transaction.Amount;
                }
                else if (transaction.TransactionType == "Expense")
                {
                    account.Balance -= transaction.Amount;
                }
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        // Updates a transaction by first reversing the old balance effect on the previous account,
        // then applying the new balance effect on the (possibly different) new account
        public async Task Update(Transaction transaction)
        {
            var oldTransaction = await _context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == transaction.Id);

            if (oldTransaction != null)
            {
                var oldAccount = await _context.Accounts.FindAsync(oldTransaction.AccountId);

                if (oldAccount != null)
                {
                    if (oldTransaction.TransactionType == "Income")
                        oldAccount.Balance -= oldTransaction.Amount;
                    else if (oldTransaction.TransactionType == "Expense")
                        oldAccount.Balance += oldTransaction.Amount;
                }
            }

            var newAccount = await _context.Accounts.FindAsync(transaction.AccountId);

            if (newAccount != null)
            {
                if (transaction.TransactionType == "Income")
                    newAccount.Balance += transaction.Amount;
                else if (transaction.TransactionType == "Expense")
                    newAccount.Balance -= transaction.Amount;
            }

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        // Deletes a transaction by ID, reverses its balance effect on the linked account,
        // and resets any bill that referenced this transaction back to unpaid
        public async Task Delete(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction != null)
            {
                var relatedBill = await _context.Bills
                    .FirstOrDefaultAsync(b => b.TransactionId == id);

                if (relatedBill != null)
                {
                    relatedBill.Paid = false;
                    relatedBill.PaidDate = null;
                    relatedBill.TransactionId = null;
                }

                var account = await _context.Accounts.FindAsync(transaction.AccountId);

                if (account != null)
                {
                    if (transaction.TransactionType == "Income")
                        account.Balance -= transaction.Amount;
                    else if (transaction.TransactionType == "Expense")
                        account.Balance += transaction.Amount;
                }

                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }

        // Filters transactions for a user by any combination of keyword (title/category/type/account/reference/notes),
        // transaction type, category, account, and date range; results are ordered by date descending
        public async Task<List<Transaction>> Search(
            int userId,
            string? keyword,
            string? type,
            string? category,
            int? accountId,
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(t =>
                    t.Title.Contains(keyword) ||
                    t.Category.Contains(keyword) ||
                    t.TransactionType.Contains(keyword) ||
                    (t.Account != null && t.Account.Name.Contains(keyword)) ||
                    (t.Reference != null && t.Reference.Contains(keyword)) ||
                    (t.Notes != null && t.Notes.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(t => t.TransactionType == type);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(t => t.Category == category);
            }

            if (accountId.HasValue)
            {
                query = query.Where(t => t.AccountId == accountId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate.Value);
            }

            return await query
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }
    }
}
