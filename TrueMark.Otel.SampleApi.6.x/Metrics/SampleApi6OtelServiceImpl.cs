using System;
using System.Diagnostics;
using TrueMark.Otel.Helper;

namespace TrueMark.Otel.SampleApi._6.x.Metrics
{
    public class SampleApi6OtelServiceImpl : SampleApi6OtelService
    {
        static readonly MetricTagHolder<long> ProcessedRequestsMetric = SampleApi6MetricsRegistry.ProcessedRequestMetric;
        static readonly MetricTagHolder<long> SuccessfulRequestsMetric = SampleApi6MetricsRegistry.SuccessfulRequestsMetric;
        static readonly MetricTagHolder<long> FailedRequestsMetric = SampleApi6MetricsRegistry.FailedRequestsMetric;
        readonly IHttpContextAccessor context;

        public SampleApi6OtelServiceImpl(IHttpContextAccessor context)
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