using System.ComponentModel.DataAnnotations.Schema;

namespace SpendSense.Models
{
    public class Bill
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public int DueDay { get; set; }

        public bool Paid { get; set; }

        public DateTime? PaidDate { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public int? AccountId { get; set; }

        public Account? Account { get; set; }

        public int? TransactionId { get; set; }

        public Transaction? Transaction { get; set; }
    }
}