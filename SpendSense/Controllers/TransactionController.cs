using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class TransactionController : Controller
    {
        private readonly TransactionService _transactionService;
        private readonly AccountService _accountService;

        // Injects TransactionService and AccountService dependencies via constructor
        public TransactionController(TransactionService transactionService, AccountService accountService)
        {
            _transactionService = transactionService;
            _accountService = accountService;
        }

        // Lists transactions for the current user, filtered by any combination of keyword, type, category,
        // account, and date range; also populates the account dropdown for the filter form
        public async Task<IActionResult> Index(string? keyword, string? type, string? category, int? accountId, DateTime? startDate, DateTime? endDate)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var transactions = await _transactionService.Search(
                userId.Value, keyword, type, category, accountId, startDate, endDate
            );

            var accounts = await _accountService.GetAllByUserId(userId.Value);

            ViewBag.Keyword = keyword;
            ViewBag.Type = type;
            ViewBag.Category = category;
            ViewBag.AccountId = accountId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Accounts = new SelectList(accounts, "Id", "Name", accountId);

            return View(transactions);
        }

        // Renders the transaction creation form with the user's accounts as a dropdown
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var accounts = await _accountService.GetAllByUserId(userId.Value);
            ViewBag.Accounts = new SelectList(accounts, "Id", "Name");

            return View();
        }

        // Saves a new transaction; if a receipt image is uploaded it is stored as a byte array
        [HttpPost]
        public async Task<IActionResult> Create(Transaction transaction, IFormFile? receiptFile)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            transaction.UserId = userId.Value;

            if (receiptFile != null && receiptFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await receiptFile.CopyToAsync(memoryStream);

                transaction.ReceiptImage = memoryStream.ToArray();
                transaction.ReceiptImageContentType = receiptFile.ContentType;
            }

            await _transactionService.Add(transaction);

            return RedirectToAction("Index");
        }

        // Renders the edit form pre-filled with the transaction's current data and account dropdown
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var transaction = await _transactionService.GetById(id);

            if (transaction == null || transaction.UserId != userId.Value)
                return NotFound();

            var accounts = await _accountService.GetAllByUserId(userId.Value);
            ViewBag.Accounts = new SelectList(accounts, "Id", "Name", transaction.AccountId);

            return View(transaction);
        }

        // Updates an existing transaction's fields, replaces the receipt image if a new file is provided,
        // and re-adjusts the linked account balance accordingly
        [HttpPost]
        public async Task<IActionResult> Edit(Transaction transaction, IFormFile? receiptFile)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var existingTransaction = await _transactionService.GetById(transaction.Id);

            if (existingTransaction == null || existingTransaction.UserId != userId.Value)
                return NotFound();

            existingTransaction.TransactionType = transaction.TransactionType;
            existingTransaction.Title = transaction.Title;
            existingTransaction.Amount = transaction.Amount;
            existingTransaction.Category = transaction.Category;
            existingTransaction.AccountId = transaction.AccountId;
            existingTransaction.Date = transaction.Date;
            existingTransaction.Reference = transaction.Reference;
            existingTransaction.Notes = transaction.Notes;

            if (receiptFile != null && receiptFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await receiptFile.CopyToAsync(memoryStream);

                existingTransaction.ReceiptImage = memoryStream.ToArray();
                existingTransaction.ReceiptImageContentType = receiptFile.ContentType;
            }

            await _transactionService.Update(existingTransaction);

            return RedirectToAction("Index");
        }

        // Shows the read-only detail view of a single transaction, including its linked account
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var transaction = await _transactionService.GetById(id);

            if (transaction == null || transaction.UserId != userId.Value)
                return NotFound();

            return View(transaction);
        }

        // Deletes a transaction by ID, reverses its effect on the account balance, and redirects to the list
        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var transaction = await _transactionService.GetById(id);

            if (transaction == null || transaction.UserId != userId.Value)
                return NotFound();

            await _transactionService.Delete(id);

            return RedirectToAction("Index");
        }

        // Serves the stored receipt image bytes as an HTTP file response so the browser can display it
        public async Task<IActionResult> ReceiptImage(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var transaction = await _transactionService.GetById(id);

            if (transaction == null || transaction.UserId != userId.Value || transaction.ReceiptImage == null)
                return NotFound();

            return File(transaction.ReceiptImage, transaction.ReceiptImageContentType ?? "image/jpeg");
        }
    }
}
