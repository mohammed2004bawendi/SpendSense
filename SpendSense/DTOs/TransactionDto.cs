namespace SpendSense.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public bool HasReceipt { get; set; }
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public string? AccountName { get; set; }
    }
}
