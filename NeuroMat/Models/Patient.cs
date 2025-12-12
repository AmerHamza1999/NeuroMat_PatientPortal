using System.ComponentModel.DataAnnotations;

namespace NeuroMat.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Notes { get; set; }


        // optional link to Identity user if you require login for patient
        public string UserId { get; set; }
        public virtual User User { get; set; }


        public ICollection<PressureFrame> PressureFrames { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
