using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;

namespace TrueMark.Otel.Helper
{
    /// <summary>
    /// Implementation of the IMetricsRegistrar interface for registering and managing metrics.
    /// </summary>
    public class MetricsRegistrarImpl : IMetricsRegistrar
    {
        private readonly ILogger<MetricsRegistrarImpl> _logger;
        private readonly Meter _meter;
        private readonly ConcurrentDictionary<string, Counter<long>> _counters = new();
        private readonly ConcurrentDictionary<string, UpDownCounter<long>> _upDownCounters = new();
        private readonly ConcurrentDictionary<string, Histogram<double>> _doubleHistograms = new();
        private readonly ConcurrentDictionary<string, Histogram<long>> _longHistograms = new();
        private readonly ConcurrentDictionary<string, ObservableGauge<long>> _longGauges = new();
        private readonly ConcurrentDictionary<string, ObservableGauge<double>> _doubleGauges = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsRegistrarImpl"/> class.
        /// </summary>
        /// <param name="meter">The meter used for creating metrics.</param>
        /// <param name="logger">The logger used for logging errors and information.</param>
        public MetricsRegistrarImpl(Meter meter, ILogger<MetricsRegistrarImpl> logger)
        {
            _meter = meter ?? throw new ArgumentNullException(nameof(meter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Registers a counter metric.
        /// </summary>
        /// <param name="counterName">The name of the counter.</param>
        /// <param name="description">The description of the counter.</param>
        /// <param name="unit">The unit of the counter.</param>
        /// <returns>The registered counter.</returns>
        public Counter<long> RegisterCounter(string counterName, string description, string unit)
        {
            return _counters.GetOrAdd(counterName, _meter.CreateCounter<long>(counterName, unit, description));
        }

        /// <summary>
        /// Registers an up-down counter metric.
        /// </summary>
        /// <param name="counterName">The name of the counter.</param>
        /// <param name="description">The description of the counter.</param>
        /// <param name="unit">The unit of the counter.</param>
        /// <returns>The registered up-down counter.</returns>
        public UpDownCounter<long> RegisterUpDownCounter(string counterName, string description, string unit)
        {
            return _upDownCounters.GetOrAdd(counterName, _meter.CreateUpDownCounter<long>(counterName, unit, description));
        }

        /// <summary>
        /// Registers a double histogram metric.
        /// </summary>
        /// <param name="histogramName">The name of the histogram.</param>
        /// <param name="description">The description of the histogram.</param>
        /// <param name="unit">The unit of the histogram.</param>
        /// <returns>The registered double histogram.</returns>
        public Histogram<double> RegisterDoubleHistogram(string histogramName, string description, string unit)
        {
            return _doubleHistograms.GetOrAdd(histogramName, _meter.CreateHistogram<double>(histogramName, unit, description));
        }

        /// <summary>
        /// Registers a long histogram metric.
        /// </summary>
        /// <param name="histogramName">The name of the histogram.</param>
        /// <param name="description">The description of the histogram.</param>
        /// <param name="unit">The unit of the histogram.</param>
        /// <returns>The registered long histogram.</returns>
        public Histogram<long> RegisterLongHistogram(string histogramName, string description, string unit)
        {
            return _longHistograms.GetOrAdd(histogramName, _meter.CreateHistogram<long>(histogramName, unit, description));
        }

        /// <summary>
        /// Registers a long gauge metric.
        /// </summary>
        /// <param name="gaugeName">The name of the gauge.</param>
        /// <param name="description">The description of the gauge.</param>
        /// <param name="unit">The unit of the gauge.</param>
        /// <param name="valueSupplier">The function that supplies the value for the gauge.</param>
        public void RegisterLongGauge(string gaugeName, string description, string unit, Func<long> valueSupplier)
        {
            var gauge = _meter.CreateObservableGauge(gaugeName, valueSupplier, unit, description);
            _longGauges.TryAdd(gaugeName, gauge);
        }

        /// <summary>
        /// Registers a double gauge metric.
        /// </summary>
        /// <param name="gaugeName">The name of the gauge.</param>
        /// <param name="description">The description of the gauge.</param>
        /// <param name="unit">The unit of the gauge.</param>
        /// <param name="valueSupplier">The function that supplies the value for the gauge.</param>
        public void RegisterDoubleGauge(string gaugeName, string description, string unit, Func<double> valueSupplier)
        {
            var gauge = _meter.CreateObservableGauge(gaugeName, valueSupplier, unit, description);
            _doubleGauges.TryAdd(gaugeName, gauge);
        }

        /// <summary>
        /// Increments a counter metric.
        /// </summary>
        /// <param name="counterName">The name of the counter.</param>
        /// <param name="value">The value to increment by.</param>
        /// <param name="attributes">The attributes associated with the counter.</param>
        public void IncrementCounter(string counterName, long value, KeyValuePair<string, object>[] attributes)
        {
            if (_counters.TryGetValue(counterName, out var counter))
            {
                counter.Add(value, attributes);
            }
            else
            {
                _logger.LogError($"Counter with name {counterName} not registered.");
                throw new InvalidOperationException($"Counter with name {counterName} not registered.");
            }
        }

        /// <summary>
        /// Updates an up-down counter metric.
        /// </summary>
        /// <param name="counterName">The name of the counter.</param>
        /// <param name="value">The value to update by.</param>
        /// <param name="attributes">The attributes associated with the counter.</param>
        public void UpdateUpDownCounter(string counterName, long value, KeyValuePair<string, object>[] attributes)
        {
            if (_upDownCounters.TryGetValue(counterName, out var upDownCounter))
            {
                upDownCounter.Add(value, attributes);
            }
            else
            {
                _logger.LogError($"UpDownCounter with name {counterName} not registered.");
                throw new InvalidOperationException($"UpDownCounter with name {counterName} not registered.");
            }
        }

        /// <summary>
        /// Records a value in a double histogram metric.
        /// </summary>
        /// <param name="histogramName">The name of the histogram.</param>
        /// <param name="value">The value to record.</param>
        /// <param name="attributes">The attributes associated with the histogram.</param>
        public void RecordHistogram(string histogramName, double value, KeyValuePair<string, object>[] attributes)
        {
            if (_doubleHistograms.TryGetValue(histogramName, out var histogram))
            {
                histogram.Record(value, attributes);
            }
            else
            {
                _logger.LogError($"Histogram with name {histogramName} not registered.");
                throw new InvalidOperationException($"Histogram with name {histogramName} not registered.");
            }
        }

        /// <summary>
        /// Gets a registered double gauge metric.
        /// </summary>
        /// <param name="gaugeName">The name of the gauge.</param>
        /// <returns>The registered double gauge.</returns>
        public ObservableGauge<double> GetDoubleGauge(string gaugeName)
        {
            _doubleGauges.TryGetValue(gaugeName, out var gauge);
            return gauge;
        }

        /// <summary>
        /// Gets a registered long gauge metric.
        /// </summary>
        /// <param name="gaugeName">The name of the gauge.</param>
        /// <returns>The registered long gauge.</returns>
        public ObservableGauge<long> GetLongGauge(string gaugeName)
        {
            _longGauges.TryGetValue(gaugeName, out var gauge);
            return gauge;
        }
    }
}