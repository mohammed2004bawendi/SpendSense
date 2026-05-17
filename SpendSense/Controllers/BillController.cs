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

        public BillController(BillService billService, AccountService accountService)
        {
            _billService = billService;
            _accountService = accountService;
        }

        /// <summary>
        /// Lists bills for the current user. Ensures the user is authenticated via session
        /// and calls BillService to retrieve user-specific records.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var bills = await _billService.GetAllByUserId(userId.Value);

            return View(bills);
        }

        /// <summary>
        /// GET: Renders the create-bill form and loads the user's accounts for association.
        /// </summary>
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

        /// <summary>
        /// POST: Creates a new bill for the current user. Sets UserId and PaidDate when needed,
        /// then persists the bill via BillService.
        /// </summary>
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

        /// <summary>
        /// GET: Loads an existing bill for editing after verifying ownership.
        /// Also prepares account select list for the view.
        /// </summary>
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

        /// <summary>
        /// POST: Updates an existing bill. Verifies the current user owns the bill and delegates
        /// persistence to the BillService.
        /// </summary>
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

        /// <summary>
        /// GET: Shows details for a single bill after verifying ownership.
        /// </summary>
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

        /// <summary>
        /// Deletes a bill after verifying user ownership and redirects back to bill index.
        /// </summary>
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