using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpendSense.DTOs;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class BillController : Controller
    {
        private readonly BillService _billService;
        private readonly AccountService _accountService;

        public BillController(BillService billService, AccountService accountService)
        {
            _billService = billService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var bills = await _billService.GetAllByUserId(userId.Value);

            var dtos = bills.Select(b => new BillDto
            {
                Id = b.Id,
                Name = b.Name,
                Amount = b.Amount,
                DueDay = b.DueDay,
                Paid = b.Paid,
                PaidDate = b.PaidDate,
                UserId = b.UserId,
                AccountId = b.AccountId,
                AccountName = b.Account?.Name,
                TransactionId = b.TransactionId
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

            return View(new CreateBillDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBillDto dto)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var bill = new Bill
            {
                Name = dto.Name,
                Amount = dto.Amount,
                DueDay = dto.DueDay,
                Paid = dto.Paid,
                PaidDate = dto.PaidDate,
                AccountId = dto.AccountId,
                UserId = userId.Value
            };

            if (bill.Paid && bill.PaidDate == null)
                bill.PaidDate = DateTime.Now;

            await _billService.Add(bill);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var bill = await _billService.GetById(id);

            if (bill == null || bill.UserId != userId.Value)
                return NotFound();

            var accounts = await _accountService.GetAllByUserId(userId.Value);
            ViewBag.Accounts = new SelectList(accounts, "Id", "Name", bill.AccountId);

            var dto = new EditBillDto
            {
                Id = bill.Id,
                Name = bill.Name,
                Amount = bill.Amount,
                DueDay = bill.DueDay,
                Paid = bill.Paid,
                PaidDate = bill.PaidDate,
                AccountId = bill.AccountId
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBillDto dto)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var bill = new Bill
            {
                Id = dto.Id,
                Name = dto.Name,
                Amount = dto.Amount,
                DueDay = dto.DueDay,
                Paid = dto.Paid,
                PaidDate = dto.PaidDate,
                AccountId = dto.AccountId,
                UserId = userId.Value
            };

            await _billService.Update(bill);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var bill = await _billService.GetById(id);

            if (bill == null || bill.UserId != userId.Value)
                return NotFound();

            var dto = new BillDto
            {
                Id = bill.Id,
                Name = bill.Name,
                Amount = bill.Amount,
                DueDay = bill.DueDay,
                Paid = bill.Paid,
                PaidDate = bill.PaidDate,
                UserId = bill.UserId,
                AccountId = bill.AccountId,
                AccountName = bill.Account?.Name,
                TransactionId = bill.TransactionId
            };

            return View(dto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var bill = await _billService.GetById(id);

            if (bill == null || bill.UserId != userId.Value)
                return NotFound();

            await _billService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
