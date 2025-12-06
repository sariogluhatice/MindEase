using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MindEase.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [Display(Name = "Appointment Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        // Foreign Key - İlişki
        [Required]
        public int UserId { get; set; }

        // Navigation Property
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}

