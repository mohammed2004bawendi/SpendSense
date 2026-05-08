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

        public TransactionController(TransactionService transactionService, AccountService accountService)
        {
            _transactionService = transactionService;
            _accountService = accountService;
        }

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