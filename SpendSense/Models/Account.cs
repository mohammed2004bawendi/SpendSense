using System.ComponentModel.DataAnnotations.Schema;

namespace SpendSense.Models
{
    public class Account
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AccountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public List<Transaction>? Transactions { get; set; }
    }
}