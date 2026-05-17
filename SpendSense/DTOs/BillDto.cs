namespace SpendSense.DTOs
{
    public class BillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int DueDay { get; set; }
        public bool Paid { get; set; }
        public DateTime? PaidDate { get; set; }
        public int UserId { get; set; }
        public int? AccountId { get; set; }
        public string? AccountName { get; set; }
        public int? TransactionId { get; set; }
    }
}
