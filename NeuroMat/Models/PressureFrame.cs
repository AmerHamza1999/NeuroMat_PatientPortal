using System.ComponentModel.DataAnnotations;

namespace NeuroMat.Models
{
    public class PressureFrame
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        // 32x32 = 1024 values (1–255)
        public byte[] Values { get; set; }

        // Extracted metrics
        public int PeakPressureIndex { get; set; }
        public double ContactAreaPercent { get; set; }
    }
}
