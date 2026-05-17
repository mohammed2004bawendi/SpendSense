using Microsoft.AspNetCore.Mvc;
using SpendSense.DTOs;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class BudgetController : Controller
    {
        private readonly BudgetService _budgetService;

        public BudgetController(BudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var budgets = await _budgetService.GetAllByUserId(userId.Value);

            var dtos = budgets.Select(b => new BudgetDto
            {
                Id = b.Id,
                Category = b.Category,
                Description = b.Description,
                LimitAmount = b.LimitAmount,
                FillClass = b.FillClass,
                SpentAmount = b.SpentAmount,
                RemainingAmount = b.RemainingAmount,
                ProgressPercentage = b.ProgressPercentage,
                StatusText = b.StatusText,
                UserId = b.UserId
            }).ToList();

            return View(dtos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            return View(new CreateBudgetDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBudgetDto dto)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var budget = new Budget
            {
                Category = dto.Category,
                Description = dto.Description,
                LimitAmount = dto.LimitAmount,
                UserId = userId.Value
            };

            await _budgetService.Add(budget);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var budget = await _budgetService.GetById(id);

            if (budget == null || budget.UserId != userId.Value)
                return NotFound();

            var dto = new EditBudgetDto
            {
                Id = budget.Id,
                Category = budget.Category,
                Description = budget.Description,
                LimitAmount = budget.LimitAmount
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBudgetDto dto)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var existingBudget = await _budgetService.GetById(dto.Id);

            if (existingBudget == null || existingBudget.UserId != userId.Value)
                return NotFound();

            existingBudget.Category = dto.Category;
            existingBudget.Description = dto.Description;
            existingBudget.LimitAmount = dto.LimitAmount;

            await _budgetService.Update(existingBudget);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var budget = await _budgetService.GetById(id);

            if (budget == null || budget.UserId != userId.Value)
                return NotFound();

            var dto = new BudgetDto
            {
                Id = budget.Id,
                Category = budget.Category,
                Description = budget.Description,
                LimitAmount = budget.LimitAmount,
                FillClass = budget.FillClass,
                SpentAmount = budget.SpentAmount,
                RemainingAmount = budget.RemainingAmount,
                ProgressPercentage = budget.ProgressPercentage,
                StatusText = budget.StatusText,
                UserId = budget.UserId
            };

            return View(dto);
        }

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
