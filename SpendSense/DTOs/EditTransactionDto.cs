using System.ComponentModel.DataAnnotations;

namespace SpendSense.DTOs
{
    public class EditTransactionDto
    {
        public int Id { get; set; }

        [Required]
        public string TransactionType { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public int AccountId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public bool HasReceipt { get; set; }
    }
}
