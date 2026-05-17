namespace SpendSense.DTOs
{
    public class BudgetDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal LimitAmount { get; set; }
        public string? FillClass { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public string? StatusText { get; set; }
        public int UserId { get; set; }
    }
}
