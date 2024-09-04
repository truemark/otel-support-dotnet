// IMetricsRegistrar.cs

using System.Diagnostics.Metrics;

namespace TrueMark.Otel.Helper
{
    public interface IMetricsRegistrar
    {
        Counter<long> RegisterCounter(string counterName, string description, string unit);
        UpDownCounter<long> RegisterUpDownCounter(string counterName, string description, string unit);
        Histogram<double> RegisterDoubleHistogram(string histogramName, string description, string unit);
        Histogram<long> RegisterLongHistogram(string histogramName, string description, string unit);
        void RegisterLongGauge(string gaugeName, string description, string unit, Func<long> valueSupplier);
        void RegisterDoubleGauge(string gaugeName, string description, string unit, Func<double> valueSupplier);
        void IncrementCounter(string counterName, long value, KeyValuePair<string, object>[] attributes);
        void UpdateUpDownCounter(string counterName, long value, KeyValuePair<string, object>[] attributes);
        void RecordHistogram(string histogramName, double value, KeyValuePair<string, object>[] attributes);
        ObservableGauge<double> GetDoubleGauge(string gaugeName);
        ObservableGauge<long> GetLongGauge(string gaugeName);
    }
}