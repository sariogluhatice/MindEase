using System.ComponentModel.DataAnnotations;

namespace MindEase.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "User type is required")]
        public string UserType { get; set; } = string.Empty;

        // Navigation Property - Bir kullanıcının birden fazla randevusu olabilir
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
