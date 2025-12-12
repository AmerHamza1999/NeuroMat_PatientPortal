namespace NeuroMat.Models
{
    public class PressureSession
    {
        public string Id { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int Duration { get; set; } 
        public double PeakPressureIndex { get; set; }
        public double ContactAreaPercent { get; set; }
        public List<PressureDataPoint> DataPoints { get; set; } = new();
    }

    public class PressureDataPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Pressure { get; set; }
    }

}
