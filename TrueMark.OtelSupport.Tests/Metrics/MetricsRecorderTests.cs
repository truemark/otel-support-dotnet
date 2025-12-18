using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Moq;
using OpenTelemetry.Metrics;
using TrueMark.OtelSupport.Metrics;

namespace TrueMark.OtelSupport.Tests.Metrics
{
    [Collection("OpenTelemetry")]
    public class MetricsRecorderTests
    {
        readonly Mock<MeterProviderBuilder> builderMock;
        readonly Mock<IHttpContextAccessor> httpContextAccessorMock;

        public MetricsRecorderTests()
        {
            builderMock = new Mock<MeterProviderBuilder>();

            // Reset static fields before each test
            OpenTelemetryExtension.IsInitialized = false;
            OpenTelemetryExtension.Meter = null;
            OpenTelemetryExtension.RegisteredMetricCounters.Clear();

            httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        }

        [Fact]
        public void NoArgConstructor_ShouldCreateRecorder()
        {
            // Arrange & Act
            var recorder = new MetricsRecorder();

            // Assert
            Assert.NotNull(recorder);
        }

        [Fact]
        public void ParameterizedConstructor_ShouldCreateRecorder()
        {
            // Arrange & Act
            var recorder = new MetricsRecorder(httpContextAccessorMock.Object);

            // Assert
            Assert.NotNull(recorder);
        }

        [Fact]
        public void Record_WithHttpContext_ShouldAddToItems()
        {
            // Arrange
            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, "TestInstrumentation");

            var httpContext = new DefaultHttpContext();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var recorder = new MetricsRecorder(httpContextAccessorMock.Object);
            var attributes = new List<KeyValuePair<string, object?>>
            {
                new KeyValuePair<string, object?>("TestLabel", "TestValue")
            };
            var tagList = new TagList(new ReadOnlySpan<KeyValuePair<string, object?>>(attributes.ToArray()));
            var metricData = new MetricTagHolder<long>("TestMetric", "Test Metric", "per request", tagList, 1);

            // Act
            recorder.Record(metricData);

            // Assert
            Assert.True(httpContext.Items.ContainsKey("TestMetric"));
            Assert.Equal(metricData, httpContext.Items["TestMetric"]);
        }

        [Fact]
        public void Record_WithoutHttpContext_ShouldRecordDirectly()
        {
            // Arrange
            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, "TestInstrumentation");

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            var recorder = new MetricsRecorder(httpContextAccessorMock.Object);
            var attributes = new List<KeyValuePair<string, object?>>
            {
                new KeyValuePair<string, object?>("TestLabel", "TestValue")
            };
            var tagList = new TagList(new ReadOnlySpan<KeyValuePair<string, object?>>(attributes.ToArray()));
            var metricData = new MetricTagHolder<long>("TestMetric", "Test Metric", "per request", tagList, 1);

            // Act
            recorder.Record(metricData);

            // Assert - Counter should be created in registry
            Assert.True(OpenTelemetryExtension.RegisteredMetricCounters.ContainsKey("TestMetric"));
        }

        [Fact]
        public void Record_NoArgConstructor_ShouldRecordDirectly()
        {
            // Arrange
            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, "TestInstrumentation");

            var recorder = new MetricsRecorder();
            var attributes = new List<KeyValuePair<string, object?>>
            {
                new KeyValuePair<string, object?>("TestLabel", "TestValue")
            };
            var tagList = new TagList(new ReadOnlySpan<KeyValuePair<string, object?>>(attributes.ToArray()));
            var metricData = new MetricTagHolder<long>("TestMetric", "Test Metric", "per request", tagList, 1);

            // Act
            recorder.Record(metricData);

            // Assert - Counter should be created in registry
            Assert.True(OpenTelemetryExtension.RegisteredMetricCounters.ContainsKey("TestMetric"));
        }

        [Fact]
        public void Record_WithoutInitialization_ShouldThrow()
        {
            // Arrange - Note: NOT calling AddMetricsServiceMeter
            var recorder = new MetricsRecorder();
            var attributes = new List<KeyValuePair<string, object?>>
            {
                new KeyValuePair<string, object?>("TestLabel", "TestValue")
            };
            var tagList = new TagList(new ReadOnlySpan<KeyValuePair<string, object?>>(attributes.ToArray()));
            var metricData = new MetricTagHolder<long>("TestMetric", "Test Metric", "per request", tagList, 1);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => recorder.Record(metricData));
        }

        [Fact]
        public void Record_WithDefaultValue_ShouldNotRecordToCounter()
        {
            // Arrange
            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, "TestInstrumentation");

            var recorder = new MetricsRecorder();
            var attributes = new List<KeyValuePair<string, object?>>
            {
                new KeyValuePair<string, object?>("TestLabel", "TestValue")
            };
            var tagList = new TagList(new ReadOnlySpan<KeyValuePair<string, object?>>(attributes.ToArray()));
            var metricData = new MetricTagHolder<long>("TestMetric", "Test Metric", "per request", tagList, 0); // Default value

            // Act
            recorder.Record(metricData);

            // Assert - Counter should still be created, but not incremented
            // We can't directly verify the counter wasn't incremented, but we can verify it was created
            Assert.True(OpenTelemetryExtension.RegisteredMetricCounters.ContainsKey("TestMetric"));
        }

        [Fact]
        public void Record_MultipleTimesWithSameMetric_ShouldReuseCounter()
        {
            // Arrange
            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, "TestInstrumentation");

            var recorder = new MetricsRecorder();
            var attributes = new List<KeyValuePair<string, object?>>
            {
                new KeyValuePair<string, object?>("TestLabel", "TestValue")
            };
            var tagList = new TagList(new ReadOnlySpan<KeyValuePair<string, object?>>(attributes.ToArray()));
            var metricData1 = new MetricTagHolder<long>("TestMetric", "Test Metric", "per request", tagList, 1);
            var metricData2 = new MetricTagHolder<long>("TestMetric", "Test Metric", "per request", tagList, 2);

            // Act
            recorder.Record(metricData1);
            var counterCountAfterFirst = OpenTelemetryExtension.RegisteredMetricCounters.Count;

            recorder.Record(metricData2);
            var counterCountAfterSecond = OpenTelemetryExtension.RegisteredMetricCounters.Count;

            // Assert - Should reuse the same counter (same count)
            Assert.Equal(1, counterCountAfterFirst);
            Assert.Equal(1, counterCountAfterSecond);
        }

        [Fact]
        public void Record_HttpContextBecomesAvailable_ShouldUseHttpContext()
        {
            // Arrange
            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, "TestInstrumentation");

            // Start with no HttpContext
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
            var recorder = new MetricsRecorder(httpContextAccessorMock.Object);

            var attributes = new List<KeyValuePair<string, object?>>
            {
                new KeyValuePair<string, object?>("TestLabel", "TestValue")
            };
            var tagList = new TagList(new ReadOnlySpan<KeyValuePair<string, object?>>(attributes.ToArray()));
            var metricData1 = new MetricTagHolder<long>("TestMetric1", "Test Metric 1", "per request", tagList, 1);

            // Act 1 - Record without HttpContext
            recorder.Record(metricData1);
            Assert.True(OpenTelemetryExtension.RegisteredMetricCounters.ContainsKey("TestMetric1"));

            // Arrange 2 - Now HttpContext becomes available
            var httpContext = new DefaultHttpContext();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            var metricData2 = new MetricTagHolder<long>("TestMetric2", "Test Metric 2", "per request", tagList, 2);

            // Act 2 - Record with HttpContext
            recorder.Record(metricData2);

            // Assert - Second metric should be in HttpContext.Items
            Assert.True(httpContext.Items.ContainsKey("TestMetric2"));
            Assert.Equal(metricData2, httpContext.Items["TestMetric2"]);
        }

        [Fact]
        public void Record_DifferentMetricTypes_ShouldHandleCorrectly()
        {
            // Arrange
            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, "TestInstrumentation");

            var recorder = new MetricsRecorder();
            var attributes = new List<KeyValuePair<string, object?>>
            {
                new KeyValuePair<string, object?>("TestLabel", "TestValue")
            };
            var tagList = new TagList(new ReadOnlySpan<KeyValuePair<string, object?>>(attributes.ToArray()));

            var longMetric = new MetricTagHolder<long>("LongMetric", "Long Metric", "count", tagList, 100L);
            var intMetric = new MetricTagHolder<int>("IntMetric", "Int Metric", "count", tagList, 50);
            var doubleMetric = new MetricTagHolder<double>("DoubleMetric", "Double Metric", "count", tagList, 99.9);

            // Act
            recorder.Record(longMetric);
            recorder.Record(intMetric);
            recorder.Record(doubleMetric);

            // Assert
            Assert.True(OpenTelemetryExtension.RegisteredMetricCounters.ContainsKey("LongMetric"));
            Assert.True(OpenTelemetryExtension.RegisteredMetricCounters.ContainsKey("IntMetric"));
            Assert.True(OpenTelemetryExtension.RegisteredMetricCounters.ContainsKey("DoubleMetric"));
            Assert.Equal(3, OpenTelemetryExtension.RegisteredMetricCounters.Count);
        }
    }
}
