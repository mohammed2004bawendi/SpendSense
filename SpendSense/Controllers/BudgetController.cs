using Microsoft.AspNetCore.Mvc;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class BudgetController : Controller
    {
        private readonly BudgetService _budgetService;

        // Injects the BudgetService dependency via constructor
        public BudgetController(BudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        // Displays all budgets for the logged-in user, each enriched with spending progress data
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var budgets = await _budgetService.GetAllByUserId(userId.Value);

            return View(budgets);
        }

        // Renders the empty budget creation form
        [HttpGet]
        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        // Saves a new budget for the current user and redirects to the list
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

        // Renders the edit form pre-filled with the budget's current data
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

        // Updates an existing budget's category, description, limit, and fill class, then redirects to the list
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

        // Shows the read-only detail view of a single budget with its progress data
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

        // Deletes a budget by ID after verifying it belongs to the current user
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
