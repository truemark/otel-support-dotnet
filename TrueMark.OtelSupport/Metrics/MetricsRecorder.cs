using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Http;

namespace TrueMark.OtelSupport.Metrics
{
    /// <summary>
    /// Records metrics for both HTTP and non-HTTP scenarios.
    /// Automatically detects HttpContext availability and routes accordingly.
    /// </summary>
    public class MetricsRecorder
    {
        private readonly IHttpContextAccessor context;
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// No-arg constructor for startup scenarios (no DI available).
        /// Will always use direct recording since no HttpContext is available.
        /// </summary>
        public MetricsRecorder()
        {
            this.context = null;
            log.Debug("=== MetricsRecorder: No-arg constructor called (startup mode) ===");
        }

        /// <summary>
        /// Constructor for dependency injection with IHttpContextAccessor.
        /// Supports both HTTP and non-HTTP scenarios.
        /// </summary>
        public MetricsRecorder(IHttpContextAccessor context)
        {
            this.context = context;
            log.Debug("=== MetricsRecorder: Constructor called with IHttpContextAccessor ===");
        }

        /// <summary>
        /// Records a metric. Automatically chooses HTTP (middleware) or non-HTTP (direct) approach.
        /// </summary>
        public void Record<T>(MetricTagHolder<T> metricData) where T : unmanaged
        {

            try
            {
                if (context?.HttpContext != null)
                {
                    // HTTP Scenario: Add to HttpContext.Items (middleware processes later)
                    log.Debug("HttpContext available - adding to Items for middleware");
                    context.HttpContext.Items.Add(metricData.Name, metricData);
                }
                else
                {
                    // Non-HTTP Scenario: Record directly to counter
                    log.Debug("No HttpContext - recording directly to counter");
                    RecordDirectly(metricData);
                }

                log.Info($"Metric queued/recorded: {metricData.Name} = {metricData.Value}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Failed to record metric: {metricData.Name}");
                throw;
            }
        }

        private void RecordDirectly<T>(MetricTagHolder<T> metricData) where T : unmanaged
        {
            if (!OpenTelemetryExtension.IsInitialized)
            {
                log.Warn("MetricsRecorder:: OTEL not initialized");
                throw new InvalidOperationException(
                    "AddMetricsServiceMeter must be called before recording metrics.");
            }

            try
            {
                log.Debug($"Creating/getting counter: {metricData.Name}");

                // Get or create the counter
                object counter;
                if (!OpenTelemetryExtension.RegisteredMetricCounters.TryGetValue(metricData.Name, out counter))
                {
                    log.Debug($"Creating new counter: {metricData.Name}");
                    counter = OpenTelemetryExtension.Meter.CreateCounter<T>(
                        metricData.Name,
                        metricData.Unit,
                        metricData.Description
                    );
                    OpenTelemetryExtension.RegisteredMetricCounters[metricData.Name] = counter;
                }

                // Cast and record
                var metricsCounter = (Counter<T>)counter;
                if (!EqualityComparer<T>.Default.Equals(metricData.Value, default(T)))
                {
                    metricsCounter.Add(metricData.Value, metricData.TagList);
                    log.Debug($"Recorded directly: {metricData.Name} = {metricData.Value}");
                }
                else
                {
                    log.Debug($"Skipped (default value): {metricData.Name}");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, $"RecordDirectly failed: {metricData.Name}");
                throw;
            }
        }
    }
}
