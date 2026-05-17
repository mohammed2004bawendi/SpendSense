using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpendSense.DTOs;
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

            var dtos = transactions.Select(t => new TransactionDto
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

            return View(dtos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var accounts = await _accountService.GetAllByUserId(userId.Value);
            ViewBag.Accounts = new SelectList(accounts, "Id", "Name");

            return View(new CreateTransactionDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionDto dto, IFormFile? receiptFile)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var transaction = new Transaction
            {
                TransactionType = dto.TransactionType,
                Title = dto.Title,
                Amount = dto.Amount,
                Category = dto.Category,
                AccountId = dto.AccountId,
                Date = dto.Date,
                Reference = dto.Reference,
                Notes = dto.Notes,
                UserId = userId.Value
            };

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

            var dto = new EditTransactionDto
            {
                Id = transaction.Id,
                TransactionType = transaction.TransactionType,
                Title = transaction.Title,
                Amount = transaction.Amount,
                Category = transaction.Category,
                AccountId = transaction.AccountId,
                Date = transaction.Date,
                Reference = transaction.Reference,
                Notes = transaction.Notes,
                HasReceipt = transaction.ReceiptImage != null
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTransactionDto dto, IFormFile? receiptFile)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var existingTransaction = await _transactionService.GetById(dto.Id);

            if (existingTransaction == null || existingTransaction.UserId != userId.Value)
                return NotFound();

            existingTransaction.TransactionType = dto.TransactionType;
            existingTransaction.Title = dto.Title;
            existingTransaction.Amount = dto.Amount;
            existingTransaction.Category = dto.Category;
            existingTransaction.AccountId = dto.AccountId;
            existingTransaction.Date = dto.Date;
            existingTransaction.Reference = dto.Reference;
            existingTransaction.Notes = dto.Notes;

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

            var dto = new TransactionDto
            {
                Id = transaction.Id,
                TransactionType = transaction.TransactionType,
                Title = transaction.Title,
                Amount = transaction.Amount,
                Category = transaction.Category,
                Date = transaction.Date,
                Reference = transaction.Reference,
                Notes = transaction.Notes,
                HasReceipt = transaction.ReceiptImage != null,
                UserId = transaction.UserId,
                AccountId = transaction.AccountId,
                AccountName = transaction.Account?.Name
            };

            return View(dto);
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
