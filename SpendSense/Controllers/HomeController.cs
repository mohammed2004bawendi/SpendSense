using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendSense.Data;
using SpendSense.DTOs;
using SpendSense.Services;
using System.Diagnostics;

namespace SpendSense.Controllers
{
    public class HomeController : Controller
    {
        private readonly TransactionService _transactionService;
        private readonly AppDbContext _context;

        public HomeController(TransactionService transactionService, AppDbContext context)
        {
            _transactionService = transactionService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var transactions = await _transactionService.GetAllByUserId(userId.Value);

            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId.Value)
                .ToListAsync();

            var bills = await _context.Bills
                .Where(b => b.UserId == userId.Value)
                .ToListAsync();

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId.Value)
                .ToListAsync();

            var totalIncome = transactions
                .Where(t => t.TransactionType == "Income")
                .Sum(t => t.Amount);

            var totalExpense = transactions
                .Where(t => t.TransactionType == "Expense")
                .Sum(t => t.Amount);

            var recentTransactions = transactions.Take(6).Select(t => new TransactionDto
            {
                Id = t.Id,
                TransactionType = t.TransactionType,
                Title = t.Title,
                Amount = t.Amount,
                Category = t.Category,
                Date = t.Date,
                Reference = t.Reference,
                Notes = t.Notes,
                HasReceipt = t.ReceiptImage != null,
                UserId = t.UserId,
                AccountId = t.AccountId,
                AccountName = t.Account?.Name
            }).ToList();

            var dashboard = new DashboardDto
            {
                Username = HttpContext.Session.GetString("Username") ?? string.Empty,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                NetBalance = totalIncome - totalExpense,
                AccountBalance = accounts.Sum(a => a.Balance),
                AccountCount = accounts.Count,
                TransactionCount = transactions.Count,
                UnpaidBills = bills.Count(b => !b.Paid),
                PaidBills = bills.Count(b => b.Paid),
                BudgetCount = budgets.Count,
                RecentTransactions = recentTransactions
            };

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
