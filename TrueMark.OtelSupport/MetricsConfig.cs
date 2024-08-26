namespace TrueMark.Otel.Helper
{
    public class MetricsConfig
    {
        public static readonly string ConfigKey = "Metrics";

        public bool OtelEnabled { get; set; }
        public bool NewRelicEnabled { get; set; }
    }
}
