
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using Moq;

namespace TrueMark.Otel.Helper.Tests
{
    public class MetricsRegistrarImplTests
    {
        private readonly Meter _realMeter;
        private readonly Mock<ILogger<MetricsRegistrarImpl>> _mockLogger;
        private readonly MetricsRegistrarImpl _metricsRegistrar;

        public MetricsRegistrarImplTests()
        {
            _realMeter = new Meter("TestMeter");
            _mockLogger = new Mock<ILogger<MetricsRegistrarImpl>>();
            _metricsRegistrar = new MetricsRegistrarImpl(_realMeter, _mockLogger.Object);
        }

        [Fact]
        public void RegisterCounter_ShouldRegisterCounter()
        {
            var counterName = "testCounter";
            var description = "Test Counter";
            var unit = "unit";
            
            var counter = _metricsRegistrar.RegisterCounter(counterName, description, unit);
            
            Assert.NotNull(counter);
        }

        [Fact]
        public void RegisterUpDownCounter_ShouldRegisterUpDownCounter()
        {
            var counterName = "testUpDownCounter";
            var description = "Test UpDown Counter";
            var unit = "unit";
            
            var upDownCounter = _metricsRegistrar.RegisterUpDownCounter(counterName, description, unit);
            
            Assert.NotNull(upDownCounter);
        }

        [Fact]
        public void RegisterDoubleHistogram_ShouldRegisterDoubleHistogram()
        {
            var histogramName = "testDoubleHistogram";
            var description = "Test Double Histogram";
            var unit = "unit";
            
            var histogram = _metricsRegistrar.RegisterDoubleHistogram(histogramName, description, unit);
            
            Assert.NotNull(histogram);
        }

        [Fact]
        public void RegisterLongHistogram_ShouldRegisterLongHistogram()
        {
            var histogramName = "testLongHistogram";
            var description = "Test Long Histogram";
            var unit = "unit";
            
            var histogram = _metricsRegistrar.RegisterLongHistogram(histogramName, description, unit);
            
            Assert.NotNull(histogram);
        }

        [Fact]
        public void RegisterLongGauge_ShouldRegisterLongGauge()
        {
            var gaugeName = "testLongGauge";
            var description = "Test Long Gauge";
            var unit = "unit";
            Func<long> valueSupplier = () => 42;
            
            _metricsRegistrar.RegisterLongGauge(gaugeName, description, unit, valueSupplier);
            
            var gauge = _metricsRegistrar.GetLongGauge(gaugeName);
            Assert.NotNull(gauge);
        }

        [Fact]
        public void RegisterDoubleGauge_ShouldRegisterDoubleGauge()
        {
            var gaugeName = "testDoubleGauge";
            var description = "Test Double Gauge";
            var unit = "unit";
            Func<double> valueSupplier = () => 42.0;
            
            _metricsRegistrar.RegisterDoubleGauge(gaugeName, description, unit, valueSupplier);
            
            var gauge = _metricsRegistrar.GetDoubleGauge(gaugeName);
            Assert.NotNull(gauge);
        }

        [Fact]
        public void IncrementCounter_ShouldIncrementCounter()
        {
            var counterName = "testCounter";
            var value = 10L;
            var attributes = new KeyValuePair<string, object>[0];
            _metricsRegistrar.RegisterCounter(counterName, "description", "unit");
            
            _metricsRegistrar.IncrementCounter(counterName, value, attributes);
        }

        [Fact]
        public void UpdateUpDownCounter_ShouldUpdateUpDownCounter()
        {
            var counterName = "testUpDownCounter";
            var value = 10L;
            var attributes = new KeyValuePair<string, object>[0];
            _metricsRegistrar.RegisterUpDownCounter(counterName, "description", "unit");
            
            _metricsRegistrar.UpdateUpDownCounter(counterName, value, attributes);
        }

        [Fact]
        public void RecordHistogram_ShouldRecordHistogram()
        {
            var histogramName = "testDoubleHistogram";
            var value = 10.0;
            var attributes = new KeyValuePair<string, object>[0];
            _metricsRegistrar.RegisterDoubleHistogram(histogramName, "description", "unit");
            
            _metricsRegistrar.RecordHistogram(histogramName, value, attributes);
            
        }

        [Fact]
        public void GetDoubleGauge_ShouldReturnDoubleGauge()
        {
            var gaugeName = "testDoubleGauge";
            var description = "Test Double Gauge";
            var unit = "unit";
            Func<double> valueSupplier = () => 42.0;
            _metricsRegistrar.RegisterDoubleGauge(gaugeName, description, unit, valueSupplier);
            
            var gauge = _metricsRegistrar.GetDoubleGauge(gaugeName);
            
            Assert.NotNull(gauge);
        }

        [Fact]
        public void GetLongGauge_ShouldReturnLongGauge()
        {
            var gaugeName = "testLongGauge";
            var description = "Test Long Gauge";
            var unit = "unit";
            Func<long> valueSupplier = () => 42;
            _metricsRegistrar.RegisterLongGauge(gaugeName, description, unit, valueSupplier);
            
            var gauge = _metricsRegistrar.GetLongGauge(gaugeName);
            
            Assert.NotNull(gauge);
        }
    }
}