using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class BillController : Controller
    {
        private readonly BillService _billService;
        private readonly AccountService _accountService;

        // Injects BillService and AccountService dependencies via constructor
        public BillController(BillService billService, AccountService accountService)
        {
            _billService = billService;
            _accountService = accountService;
        }

        // Displays all bills belonging to the logged-in user
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var bills = await _billService.GetAllByUserId(userId.Value);

            return View(bills);
        }

        // Renders the bill creation form with the user's accounts as a dropdown
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

        // Saves a new bill; auto-sets PaidDate if the bill is marked paid on creation
        [HttpPost]
        public async Task<IActionResult> Create(Bill bill)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            bill.UserId = userId.Value;

            if (bill.Paid && bill.PaidDate == null)
                bill.PaidDate = DateTime.Now;

            await _billService.Add(bill);

            return RedirectToAction("Index");
        }

        // Renders the edit form pre-filled with the bill's current data and account dropdown
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

            return View(bill);
        }

        // Updates a bill and handles paid/unpaid state transitions (creates or removes linked transactions)
        [HttpPost]
        public async Task<IActionResult> Edit(Bill bill)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            bill.UserId = userId.Value;

            await _billService.Update(bill);

            return RedirectToAction("Index");
        }

        // Shows the read-only detail view of a single bill, including its linked account
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var bill = await _billService.GetById(id);

            if (bill == null || bill.UserId != userId.Value)
                return NotFound();

            return View(bill);
        }

        // Deletes a bill by ID and its linked transaction (if any) after verifying ownership
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
