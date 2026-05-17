using System.ComponentModel.DataAnnotations;

namespace SpendSense.DTOs
{
    public class EditAccountDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string AccountType { get; set; } = string.Empty;

        [Required]
        public decimal Balance { get; set; }
    }
}
