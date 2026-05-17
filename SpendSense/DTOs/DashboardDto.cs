namespace SpendSense.DTOs
{
    public class DashboardDto
    {
        public string Username { get; set; } = string.Empty;
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetBalance { get; set; }
        public decimal AccountBalance { get; set; }
        public int AccountCount { get; set; }
        public int TransactionCount { get; set; }
        public int UnpaidBills { get; set; }
        public int PaidBills { get; set; }
        public int BudgetCount { get; set; }
        public List<TransactionDto> RecentTransactions { get; set; } = new();
    }
}
