using System;
using System.Diagnostics;
using TrueMark.Otel.Helper;

namespace TrueMark.Otel.SampleApi._6.x.Metrics
{
    public class SampleApi8OtelServiceImpl : SampleApi8OtelService
    {
        private static readonly MetricTagHolder<long> ProcessedRequestsMetric = SampleApi8MetricsRegistry.ProcessedRequestMetric;
        private static readonly MetricTagHolder<long> SuccessfulRequestsMetric = SampleApi8MetricsRegistry.SuccessfulRequestsMetric;
        private static readonly MetricTagHolder<long> FailedRequestsMetric = SampleApi8MetricsRegistry.FailedRequestsMetric;
        private readonly IHttpContextAccessor context;

        public SampleApi8OtelServiceImpl(IHttpContextAccessor context)
        {
            this.context = context;
        }
        public void LogTestProcessedRequest()
        {
            LogMetric(ProcessedRequestsMetric);
        }

        public void LogTestSuccessfulRequest()
        {
            LogMetric(SuccessfulRequestsMetric);
        }

        public void LogTestFailedRequest()
        {
            LogMetric(FailedRequestsMetric);
        }
        
        private void LogMetric(MetricTagHolder<long> metric)
        {
            var metricData = new MetricTagHolder<long>(
                metric.Name,
                metric.Description,
                metric.Unit,
                new TagList(),
                1);
            context.HttpContext?.Items.Add(metricData.Name, metricData);
        }
    }
}