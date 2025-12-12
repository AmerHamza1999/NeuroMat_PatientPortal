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


        // store as binary compressed bytes; alternative: string JSON
        public byte[] ValuesCompressed { get; set; }


        // extracted metrics
        public int? PeakPressureIndex { get; set; }
        public double? ContactAreaPercent { get; set; }


        public bool HasAlert { get; set; }
        public string AlertReason { get; set; } 
    }
}
