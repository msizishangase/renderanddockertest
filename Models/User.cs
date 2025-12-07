using System.ComponentModel.DataAnnotations;

namespace AccountAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
