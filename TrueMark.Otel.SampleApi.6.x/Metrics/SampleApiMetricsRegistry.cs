using TrueMark.Otel.Helper;

namespace TrueMark.Otel.SampleApi._6.x.Metrics
{
    public class SampleApiMetricsRegistry : IMetricsRegistry
    {
        public static readonly MetricTagHolder<long> ProcessedRequestMetric = new MetricTagHolder<long>("processed_request", "All processed requests", "per request");
        public static readonly MetricTagHolder<long> SuccessfulRequestsMetric = new MetricTagHolder<long>("successful_request", "Successful Requests", "per request");
        public static readonly MetricTagHolder<long> FailedRequestsMetric = new MetricTagHolder<long>("failed_request", "Failed Request", "per request");
        
        public void LoadMetrics(IApplicationBuilder app)
        {
            var metricsTagsLongType = new List<MetricTagHolder<long>>
            {
                ProcessedRequestMetric,
                SuccessfulRequestsMetric,
                FailedRequestsMetric
            };
            app.UseOpenTelemetry(metricsTagsLongType);
        }
    }
}
