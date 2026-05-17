using Microsoft.AspNetCore.Mvc;
using SpendSense.DTOs;
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

            var dtos = accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                Name = a.Name,
                AccountType = a.AccountType,
                Balance = a.Balance,
                UserId = a.UserId
            }).ToList();

            return View(dtos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            return View(new CreateAccountDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountDto dto)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var account = new Account
            {
                Name = dto.Name,
                AccountType = dto.AccountType,
                Balance = dto.Balance,
                UserId = userId.Value
            };

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

            var dto = new EditAccountDto
            {
                Id = account.Id,
                Name = account.Name,
                AccountType = account.AccountType,
                Balance = account.Balance
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditAccountDto dto)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var existingAccount = await _accountService.GetById(dto.Id);

            if (existingAccount == null || existingAccount.UserId != userId.Value)
                return NotFound();

            existingAccount.Name = dto.Name;
            existingAccount.AccountType = dto.AccountType;
            existingAccount.Balance = dto.Balance;

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

            var dto = new AccountDto
            {
                Id = account.Id,
                Name = account.Name,
                AccountType = account.AccountType,
                Balance = account.Balance,
                UserId = account.UserId
            };

            return View(dto);
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
