using System.ComponentModel.DataAnnotations;

namespace NeuroMat.Models
{
    public class Alert
    {
        [Key]
        public Guid Id { get; set; }


        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }


        public Guid PressureFrameId { get; set; }
        public PressureFrame PressureFrame { get; set; }


        // Severity could be Low / Medium / High
        public string Severity { get; set; }


        // When the alert was triggered
        public DateTimeOffset CreatedAt { get; set; }


        // Whether clinician handled it
        public bool Resolved { get; set; }


        // Clinician assigned to follow up (optional)
        public string ClinicianId { get; set; }
        public User Clinician { get; set; }


        // Optional notes by clinician
        public string Notes { get; set; }
    }
}
