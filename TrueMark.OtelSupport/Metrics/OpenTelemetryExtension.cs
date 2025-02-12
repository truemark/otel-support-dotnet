using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using OpenTelemetry.Metrics;
using NLog;

namespace TrueMark.OtelSupport.Metrics
{
    /// <summary>
    /// Provides extension methods for configuring OpenTelemetry metrics in an ASP.NET Core application.
    /// </summary>
    public static class OpenTelemetryExtension
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static readonly object lockObject = new object();

        /// <summary>
        /// Gets or sets the <see cref="Meter"/> used for recording metrics.
        /// </summary>
        public static Meter Meter { get; set; }

        /// <summary>
        /// Gets the dictionary of registered metric counters.
        /// </summary>
        public static readonly Dictionary<string, object> RegisteredMetricCounters = new Dictionary<string, object>();

        /// <summary>
        /// Gets a value indicating whether the metrics service meter has been initialized.
        /// </summary>
        public static bool IsInitialized;

        /// <summary>
        /// Adds a metrics service meter to the <see cref="MeterProviderBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="MeterProviderBuilder"/> to add the meter to.</param>
        /// <param name="instrumentationName">The name of the instrumentation.</param>
        /// <param name="httpMetricsOverrideEnabled">A value indicating whether HTTP metrics override is enabled.</param>
        /// <returns>The <see cref="MeterProviderBuilder"/> with the added meter.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the method is called more than once.</exception>
        public static MeterProviderBuilder AddMetricsServiceMeter(this MeterProviderBuilder builder, string instrumentationName, bool httpMetricsOverrideEnabled = false)
        {
            lock (lockObject) // Lock this block to prevent multiple calls to AddMetricsServiceMeter
            {
                if (IsInitialized)
                {
                    throw new InvalidOperationException("AddMetricsServiceMeter can only be called once.");
                }

                Meter = new Meter(instrumentationName);
                builder.AddMeter(instrumentationName);
                if (httpMetricsOverrideEnabled)
                {
                    builder.AddMeter(HttpMetricsRecorder.TrueMarkHttpMetrics);
                }
                IsInitialized = true;
            }
            return builder;
        }

        /// <summary>
        /// Configures the application to use OpenTelemetry metrics.
        /// </summary>
        /// <typeparam name="T">The type of the value associated with the metric tag.</typeparam>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <param name="metricsTags">The list of metric tags to use.</param>
        /// <returns>The configured <see cref="IApplicationBuilder"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the metrics service meter has not been initialized.</exception>
        public static IApplicationBuilder UseOpenTelemetry<T>(this IApplicationBuilder app, List<MetricTagHolder<T>> metricsTags) where T : unmanaged
        {
            if (!IsInitialized)
            {
                logger.Warn("OTEL-Ext:: Not initialized");
                throw new InvalidOperationException("AddMetricsServiceMeter must be called before UseOpenTelemetry.");
            }
            logger.Debug("OTEL-Ext:: metricsTags: {0}", metricsTags.Count);

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
                                counter = Meter.CreateCounter<T>(metricsTag.Name, metricsTag.Unit, metricsTag.Description);
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