using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace TrueMark.OtelSupport.Metrics
{
    /// <summary>
    /// Interface for registering and managing various types of metrics.
    /// </summary>
    public interface IMetricsRegistrar
    {
        /// <summary>
        /// Registers a counter metric.
        /// </summary>
        /// <param name="counterName">The name of the counter.</param>
        /// <param name="description">The description of the counter.</param>
        /// <param name="unit">The unit of the counter.</param>
        /// <returns>A <see cref="Counter{T}"/> for the specified counter.</returns>
        Counter<long> RegisterCounter(string counterName, string description, string unit);

        /// <summary>
        /// Registers an up-down counter metric.
        /// </summary>
        /// <param name="counterName">The name of the up-down counter.</param>
        /// <param name="description">The description of the up-down counter.</param>
        /// <param name="unit">The unit of the up-down counter.</param>
        /// <returns>An <see cref="UpDownCounter{T}"/> for the specified up-down counter.</returns>
        UpDownCounter<long> RegisterUpDownCounter(string counterName, string description, string unit);

        /// <summary>
        /// Registers a histogram metric for double values.
        /// </summary>
        /// <param name="histogramName">The name of the histogram.</param>
        /// <param name="description">The description of the histogram.</param>
        /// <param name="unit">The unit of the histogram.</param>
        /// <returns>A <see cref="Histogram{T}"/> for the specified histogram.</returns>
        Histogram<double> RegisterDoubleHistogram(string histogramName, string description, string unit);

        /// <summary>
        /// Registers a histogram metric for long values.
        /// </summary>
        /// <param name="histogramName">The name of the histogram.</param>
        /// <param name="description">The description of the histogram.</param>
        /// <param name="unit">The unit of the histogram.</param>
        /// <returns>A <see cref="Histogram{T}"/> for the specified histogram.</returns>
        Histogram<long> RegisterLongHistogram(string histogramName, string description, string unit);

        /// <summary>
        /// Registers a gauge metric for long values.
        /// </summary>
        /// <param name="gaugeName">The name of the gauge.</param>
        /// <param name="description">The description of the gauge.</param>
        /// <param name="unit">The unit of the gauge.</param>
        /// <param name="valueSupplier">A function that supplies the value of the gauge.</param>
        void RegisterLongGauge(string gaugeName, string description, string unit, Func<long> valueSupplier);

        /// <summary>
        /// Registers a gauge metric for double values.
        /// </summary>
        /// <param name="gaugeName">The name of the gauge.</param>
        /// <param name="description">The description of the gauge.</param>
        /// <param name="unit">The unit of the gauge.</param>
        /// <param name="valueSupplier">A function that supplies the value of the gauge.</param>
        void RegisterDoubleGauge(string gaugeName, string description, string unit, Func<double> valueSupplier);

        /// <summary>
        /// Increments a counter metric.
        /// </summary>
        /// <param name="counterName">The name of the counter.</param>
        /// <param name="value">The value to increment the counter by.</param>
        /// <param name="attributes">The attributes associated with the counter.</param>
        void IncrementCounter(string counterName, long value, KeyValuePair<string, object>[] attributes);

        /// <summary>
        /// Updates an up-down counter metric.
        /// </summary>
        /// <param name="counterName">The name of the up-down counter.</param>
        /// <param name="value">The value to update the up-down counter by.</param>
        /// <param name="attributes">The attributes associated with the up-down counter.</param>
        void UpdateUpDownCounter(string counterName, long value, KeyValuePair<string, object>[] attributes);

        /// <summary>
        /// Records a value in a histogram metric.
        /// </summary>
        /// <param name="histogramName">The name of the histogram.</param>
        /// <param name="value">The value to record in the histogram.</param>
        /// <param name="attributes">The attributes associated with the histogram.</param>
        void RecordHistogram(string histogramName, double value, KeyValuePair<string, object>[] attributes);

        /// <summary>
        /// Gets an observable gauge metric for double values.
        /// </summary>
        /// <param name="gaugeName">The name of the gauge.</param>
        /// <returns>An <see cref="ObservableGauge{T}"/> for the specified gauge.</returns>
        ObservableGauge<double> GetDoubleGauge(string gaugeName);

        /// <summary>
        /// Gets an observable gauge metric for long values.
        /// </summary>
        /// <param name="gaugeName">The name of the gauge.</param>
        /// <returns>An <see cref="ObservableGauge{T}"/> for the specified gauge.</returns>
        ObservableGauge<long> GetLongGauge(string gaugeName);
    }
}