using System;
using System.Diagnostics;
using TrueMark.Otel.Helper;

namespace TrueMark.Otel.SampleApi._6.x.Metrics
{
    public class SampleApiOtelServiceImpl : SampleApiOtelService
    {
        static readonly MetricTagHolder<long> ProcessedRequestsMetric = SampleApiMetricsRegistry.ProcessedRequestMetric;
        static readonly MetricTagHolder<long> SuccessfulRequestsMetric = SampleApiMetricsRegistry.SuccessfulRequestsMetric;
        static readonly MetricTagHolder<long> FailedRequestsMetric = SampleApiMetricsRegistry.FailedRequestsMetric;
        readonly IHttpContextAccessor context;

        public SampleApiOtelServiceImpl(IHttpContextAccessor context)
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