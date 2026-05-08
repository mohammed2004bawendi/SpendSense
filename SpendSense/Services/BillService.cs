using Microsoft.EntityFrameworkCore;
using SpendSense.Data;
using SpendSense.Models;

namespace SpendSense.Services
{
    public class BillService
    {
        private readonly AppDbContext _context;
        private readonly TransactionService _transactionService;

        public BillService(AppDbContext context, TransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        public async Task<List<Bill>> GetAllByUserId(int userId)
        {
            return await _context.Bills
                .Include(b => b.Account)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<Bill?> GetById(int id)
        {
            return await _context.Bills
                .Include(b => b.Account)
                .Include(b => b.Transaction)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task Add(Bill bill)
        {
            if (bill.Paid && bill.AccountId.HasValue)
            {
                if (bill.PaidDate == null)
                    bill.PaidDate = DateTime.Now;

                var transaction = CreateBillTransaction(bill);

                await _transactionService.Add(transaction);

                bill.TransactionId = transaction.Id;
            }

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Bill bill)
        {
            var oldBill = await _context.Bills
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bill.Id);

            if (oldBill == null)
                return;

            bill.TransactionId = oldBill.TransactionId;

            // Paid -> Unpaid
            if (oldBill.Paid && !bill.Paid)
            {
                int? oldTransactionId = oldBill.TransactionId;

                bill.TransactionId = null;
                bill.PaidDate = null;

                _context.Bills.Update(bill);
                await _context.SaveChangesAsync();

                if (oldTransactionId.HasValue)
                {
                    await _transactionService.Delete(oldTransactionId.Value);
                }

                return;
            }

            // Unpaid -> Paid
            if (!oldBill.Paid && bill.Paid)
            {
                if (bill.PaidDate == null)
                    bill.PaidDate = DateTime.Now;

                if (bill.AccountId.HasValue)
                {
                    var transaction = CreateBillTransaction(bill);

                    await _transactionService.Add(transaction);

                    bill.TransactionId = transaction.Id;
                }

                _context.Bills.Update(bill);
                await _context.SaveChangesAsync();

                return;
            }

            // Paid -> Paid, but amount/account/date/name may change
            if (oldBill.Paid && bill.Paid)
            {
                if (bill.PaidDate == null)
                    bill.PaidDate = DateTime.Now;

                if (oldBill.TransactionId.HasValue && bill.AccountId.HasValue)
                {
                    var transaction = await _context.Transactions
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t => t.Id == oldBill.TransactionId.Value);

                    if (transaction != null)
                    {
                        transaction.Title = "Bill Payment: " + bill.Name;
                        transaction.Amount = bill.Amount;
                        transaction.Category = "Bills";
                        transaction.AccountId = bill.AccountId.Value;
                        transaction.Date = bill.PaidDate.Value;
                        transaction.Reference = "Auto-created from bill";
                        transaction.Notes = "This transaction was automatically updated from bill.";
                        transaction.UserId = bill.UserId;
                        transaction.TransactionType = "Expense";

                        await _transactionService.Update(transaction);

                        bill.TransactionId = transaction.Id;
                    }
                }

                _context.Bills.Update(bill);
                await _context.SaveChangesAsync();

                return;
            }

            // Unpaid -> Unpaid
            bill.TransactionId = null;
            bill.PaidDate = null;

            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var bill = await _context.Bills
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null)
                return;

            int? transactionId = bill.TransactionId;

         
            bill.TransactionId = null;

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            if (transactionId.HasValue)
            {
                await _transactionService.Delete(transactionId.Value);
            }
        }

        private Transaction CreateBillTransaction(Bill bill)
        {
            return new Transaction
            {
                TransactionType = "Expense",
                Title = "Bill Payment: " + bill.Name,
                Amount = bill.Amount,
                Category = "Bills",
                AccountId = bill.AccountId!.Value,
                Date = bill.PaidDate ?? DateTime.Now,
                Reference = "Auto-created from bill",
                Notes = "This transaction was automatically created when the bill was marked as paid.",
                UserId = bill.UserId
            };
        }
    }
}