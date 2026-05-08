using System.ComponentModel.DataAnnotations;

namespace SpendSense.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime DateOfBirth { get; set; }

        public List<Transaction>? Transactions { get; set; }
    }
}