namespace NeuroMat.Services
{
    public record PressureMetrics(
       int PeakPressureIndex,
       double ContactAreaPercent
    );

    public interface IPressureAnalysisService
    {
        PressureMetrics Analyze(byte[] values);
    }
    public class PressureAnalysisService : IPressureAnalysisService
    {
        private const int Size = 32;
        private const int ContactThreshold = 10;

        public PressureMetrics Analyze(byte[] values)
        {
            int peak = 0;
            int contactPixels = 0;

            for (int i = 0; i < values.Length; i++)
            {
                byte v = values[i];

                if (v > peak)
                    peak = v;

                if (v > ContactThreshold)
                    contactPixels++;
            }

            double contactAreaPercent =
                (contactPixels / (double)(Size * Size)) * 100.0;

            return new PressureMetrics(
                peak,
                Math.Round(contactAreaPercent, 2)
            );
        }
    }
}
