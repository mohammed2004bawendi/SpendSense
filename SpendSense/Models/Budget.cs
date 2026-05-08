using System.ComponentModel.DataAnnotations.Schema;

namespace SpendSense.Models
{
    public class Budget
    {
        public int Id { get; set; }

        public string Category { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LimitAmount { get; set; }

        public string? FillClass { get; set; }

        [NotMapped]
        public decimal SpentAmount { get; set; }

        [NotMapped]
        public decimal RemainingAmount { get; set; }

        [NotMapped]
        public decimal ProgressPercentage { get; set; }

        [NotMapped]
        public string StatusText { get; set; } = "Good";

        public int UserId { get; set; }

        public User? User { get; set; }
    }
}