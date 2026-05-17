using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendSense.Data;
using SpendSense.Models;
using SpendSense.Services;
using System.Diagnostics;

namespace SpendSense.Controllers
{
    public class HomeController : Controller
    {
        private readonly TransactionService _transactionService;
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor - injects TransactionService and AppDbContext.
        /// TransactionService provides business logic for transaction data; AppDbContext
        /// is used directly here for quick access to accounts, bills and budgets for the dashboard.
        /// </summary>
        public HomeController(TransactionService transactionService, AppDbContext context)
        {
            _transactionService = transactionService;
            _context = context;
        }

        /// <summary>
        /// Renders the dashboard (home) view. Gathers transactions, accounts, bills and budgets
        /// for the current user and computes summary statistics (income, expense, balances, counts)
        /// using LINQ and passes recent transactions to the view.
        /// </summary>
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

            ViewBag.Username = HttpContext.Session.GetString("Username");

            ViewBag.TotalIncome = transactions
                .Where(t => t.TransactionType == "Income")
                .Sum(t => t.Amount);

            ViewBag.TotalExpense = transactions
                .Where(t => t.TransactionType == "Expense")
                .Sum(t => t.Amount);

            ViewBag.Balance = ViewBag.TotalIncome - ViewBag.TotalExpense;

            ViewBag.AccountBalance = accounts.Sum(a => a.Balance);
            ViewBag.AccountCount = accounts.Count;
            ViewBag.TransactionCount = transactions.Count;
            ViewBag.UnpaidBills = bills.Count(b => !b.Paid);
            ViewBag.PaidBills = bills.Count(b => b.Paid);
            ViewBag.BudgetCount = budgets.Count;

            var recentTransactions = transactions
                .Take(6)
                .ToList();

            return View(recentTransactions);
        }

        /// <summary>
        /// Returns the Privacy view.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Error action used by the global exception handler to display an error page.
        /// ResponseCache attributes prevent caching of error responses.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}