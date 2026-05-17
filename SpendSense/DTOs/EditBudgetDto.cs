using System.ComponentModel.DataAnnotations;

namespace SpendSense.DTOs
{
    public class EditBudgetDto
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public decimal LimitAmount { get; set; }
    }
}
