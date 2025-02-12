namespace TrueMark.OtelSupport.Metrics
{
    /// <summary>
    /// Configuration settings for metrics.
    /// </summary>
    public class MetricsConfig
    {
        /// <summary>
        /// The configuration key for metrics settings.
        /// </summary>
        public static readonly string ConfigKey = "Metrics";

        /// <summary>
        /// Gets or sets a value indicating whether OpenTelemetry is enabled.
        /// </summary>
        public bool OtelEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether New Relic is enabled.
        /// </summary>
        public bool NewRelicEnabled { get; set; }
    }
}