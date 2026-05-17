using System.ComponentModel.DataAnnotations;

namespace SpendSense.DTOs
{
    public class EditBillDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [Range(1, 31)]
        public int DueDay { get; set; }

        public bool Paid { get; set; }
        public DateTime? PaidDate { get; set; }
        public int? AccountId { get; set; }
    }
}
