using Microsoft.AspNetCore.Mvc;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var accounts = await _accountService.GetAllByUserId(userId.Value);

            return View(accounts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Account account)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            account.UserId = userId.Value;

            await _accountService.Add(account);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var account = await _accountService.GetById(id);

            if (account == null || account.UserId != userId.Value)
                return NotFound();

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Account account)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var existingAccount = await _accountService.GetById(account.Id);

            if (existingAccount == null || existingAccount.UserId != userId.Value)
                return NotFound();

            existingAccount.Name = account.Name;
            existingAccount.AccountType = account.AccountType;
            existingAccount.Balance = account.Balance;

            await _accountService.Update(existingAccount);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var account = await _accountService.GetById(id);

            if (account == null || account.UserId != userId.Value)
                return NotFound();

            return View(account);
        }

        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var account = await _accountService.GetById(id);

            if (account == null || account.UserId != userId.Value)
                return NotFound();

            await _accountService.Delete(id);

            return RedirectToAction("Index");
        }
    }
}