using Microsoft.EntityFrameworkCore;
using SpendSense.Data;
using SpendSense.Models;

namespace SpendSense.Services
{
    public class BudgetService
    {
        private readonly AppDbContext _context;

        public BudgetService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Budget>> GetAllByUserId(int userId)
        {
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId)
                .ToListAsync();

            foreach (var budget in budgets)
            {
                await FillBudgetProgress(budget, userId);
            }

            return budgets;
        }

        public async Task<Budget?> GetById(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);

            if (budget != null)
            {
                await FillBudgetProgress(budget, budget.UserId);
            }

            return budget;
        }

        public async Task Add(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);

            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }
        }

        private async Task FillBudgetProgress(Budget budget, int userId)
        {
            var spent = await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.TransactionType == "Expense" &&
                    t.Category == budget.Category)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            budget.SpentAmount = spent;
            budget.RemainingAmount = budget.LimitAmount - spent;

            if (budget.LimitAmount > 0)
                budget.ProgressPercentage = Math.Round((spent / budget.LimitAmount) * 100, 2);
            else
                budget.ProgressPercentage = 0;

            if (budget.ProgressPercentage >= 100)
            {
                budget.FillClass = "danger";
                budget.StatusText = "Over Budget";
            }
            else if (budget.ProgressPercentage >= 80)
            {
                budget.FillClass = "warning";
                budget.StatusText = "Warning";
            }
            else
            {
                budget.FillClass = "success";
                budget.StatusText = "Good";
            }
        }
    }
}