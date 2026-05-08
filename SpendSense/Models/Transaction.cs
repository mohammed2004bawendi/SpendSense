using System.ComponentModel.DataAnnotations.Schema;

namespace SpendSense.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public string TransactionType { get; set; }

        public string Title { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string Category { get; set; }

        public DateTime Date { get; set; }

        public string? Reference { get; set; }

        public string? Notes { get; set; }

        public byte[]? ReceiptImage { get; set; }

        public string? ReceiptImageContentType { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public int AccountId { get; set; }

        public Account? Account { get; set; }
    }
}