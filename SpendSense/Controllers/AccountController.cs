using Microsoft.AspNetCore.Mvc;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        /// <summary>
        /// Constructor - injects AccountService used to perform account-related business logic
        /// and data access. Services are provided by the DI container at runtime.
        /// </summary>
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Lists all accounts for the currently authenticated user.
        /// Verifies session-based authentication and calls the service to retrieve user accounts.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var accounts = await _accountService.GetAllByUserId(userId.Value);

            return View(accounts);
        }

        /// <summary>
        /// GET: Renders the create-account form for authenticated users.
        /// Ensures the user is signed in via session before rendering.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        /// <summary>
        /// POST: Creates a new account for the current user.
        /// Binds form data to an Account model, sets the UserId from session and persists via service.
        /// </summary>
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

        /// <summary>
        /// GET: Loads an existing account for editing.
        /// Validates ownership by comparing the session UserId and redirects or returns NotFound if invalid.
        /// </summary>
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

        /// <summary>
        /// POST: Updates an existing account. Verifies ownership, copies editable fields
        /// and calls the AccountService to persist changes.
        /// </summary>
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

        /// <summary>
        /// GET: Shows details for a specific account after verifying user ownership.
        /// </summary>
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

        /// <summary>
        /// Deletes an account by id after verifying that the current user owns it.
        /// Delegates the deletion to the AccountService and redirects to the account list.
        /// </summary>
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