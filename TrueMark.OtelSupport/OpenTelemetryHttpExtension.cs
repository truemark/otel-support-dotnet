using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using OpenTelemetry.Metrics;

namespace TrueMark.Otel.Helper
{
    public static class OpenTelemetryHttpExtension
    {
        private static readonly object lockObject = new object();
        public static Meter? Meter { get; set; }
        public static readonly Dictionary<string, object> RegisteredMetricCounters = new Dictionary<string, object>();
        public static bool IsInitialized;

        public static MeterProviderBuilder AddMetricsServiceMeter(this MeterProviderBuilder builder, string instrumentationName)
        {
            lock (lockObject) // Lock this block to prevent multiple calls to AddMetricsServiceMeter
            {
                if (IsInitialized)
                {
                    throw new InvalidOperationException("AddMetricsServiceMeter can only be called once.");
                }

                Meter = new Meter(instrumentationName);
                builder.AddMeter(instrumentationName);
                IsInitialized = true;
            }
            return builder;
        }

        public static IApplicationBuilder UseOpenTelemetry<T>(this IApplicationBuilder app, List<MetricTagHolder<T>> metricsTags) where T : unmanaged
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("AddMetricsServiceMeter must be called before UseOpenTelemetry.");
            }

            app.Use(async (context, next) =>
            {
                await next();

                foreach (var metricsTag in metricsTags)
                {
                    if (!context.Items.TryGetValue(metricsTag.Name, out var value) || !(value is MetricTagHolder<T> metricsMetadata))
                    {
                        continue;
                    }

                    if (!RegisteredMetricCounters.TryGetValue(metricsTag.Name, out var counter))
                    {
                        lock (lockObject) // Lock this block to insert the counter to the dictionary
                        {
                            if (!RegisteredMetricCounters.TryGetValue(metricsTag.Name, out counter))
                            {
                                counter = Meter!.CreateCounter<T>(metricsTag.Name, metricsTag.Unit, metricsTag.Description);
                                RegisteredMetricCounters[metricsTag.Name] = counter;
                            }
                        }
                    }

                    var metricsCounter = (Counter<T>)counter;
                    if (!EqualityComparer<T>.Default.Equals(metricsMetadata.Value, default(T)))
                    {
                        metricsCounter.Add(metricsMetadata.Value, metricsMetadata.TagList);
                    }
                }
            });

            return app;
        }
    }
}
