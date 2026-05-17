using Microsoft.AspNetCore.Mvc;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class BudgetController : Controller
    {
        private readonly BudgetService _budgetService;

        /// <summary>
        /// Constructor - injects BudgetService for budget-related operations and data access.
        /// The service will be provided by DI at runtime.
        /// </summary>
        public BudgetController(BudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        /// <summary>
        /// Lists all budgets for the current user by calling the BudgetService.
        /// Ensures user is authenticated via session before retrieving data.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var budgets = await _budgetService.GetAllByUserId(userId.Value);

            return View(budgets);
        }

        /// <summary>
        /// GET: Renders the create-budget form for authenticated users.
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
        /// POST: Creates a new budget for the current user. Sets UserId from session and
        /// calls BudgetService.Add to persist the new budget entity.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(Budget budget)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            budget.UserId = userId.Value;

            await _budgetService.Add(budget);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: Loads an existing budget for editing after checking ownership.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var budget = await _budgetService.GetById(id);

            if (budget == null || budget.UserId != userId.Value)
                return NotFound();

            return View(budget);
        }

        /// <summary>
        /// POST: Updates an existing budget. Verifies ownership, copies updateable fields,
        /// and delegates persistence to BudgetService.Update.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(Budget budget)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var existingBudget = await _budgetService.GetById(budget.Id);

            if (existingBudget == null || existingBudget.UserId != userId.Value)
                return NotFound();

            existingBudget.Category = budget.Category;
            existingBudget.Description = budget.Description;
            existingBudget.LimitAmount = budget.LimitAmount;
            existingBudget.FillClass = budget.FillClass;

            await _budgetService.Update(existingBudget);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: Shows details for a specific budget after verifying the current user's ownership.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var budget = await _budgetService.GetById(id);

            if (budget == null || budget.UserId != userId.Value)
                return NotFound();

            return View(budget);
        }

        /// <summary>
        /// Deletes a budget after verifying ownership, then redirects to the budget list.
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var budget = await _budgetService.GetById(id);

            if (budget == null || budget.UserId != userId.Value)
                return NotFound();

            await _budgetService.Delete(id);

            return RedirectToAction("Index");
        }
    }
}