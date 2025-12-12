using System.ComponentModel.DataAnnotations;

namespace NeuroMat.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Patient name is required")]
        public string DisplayName { get; set; } = string.Empty;

        public string? Notes { get; set; }

        // optional link to Identity user
        public string? UserId { get; set; }
        public virtual User? User { get; set; }

        public ICollection<PressureFrame> PressureFrames { get; set; }
            = new List<PressureFrame>();
    }
}
