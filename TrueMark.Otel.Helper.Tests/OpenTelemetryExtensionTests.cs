using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using OpenTelemetry.Metrics;

namespace TrueMark.Otel.Helper.Tests
{
    public class OpenTelemetryExtensionTests
    {
        readonly Mock<MeterProviderBuilder> builderMock;

        public OpenTelemetryExtensionTests()
        {
            builderMock = new Mock<MeterProviderBuilder>();

            // Reset static fields before each test
            OpenTelemetryExtension.IsInitialized = false;
            OpenTelemetryExtension.Meter = null;
            OpenTelemetryExtension.registeredMetricCounters.Clear();
        }

        [Fact]
        public void AddMetricsServiceMeter_ShouldInitializeMeter()
        {
            var instrumentationName = "TestInstrumentation";

            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, instrumentationName);

            Assert.True(OpenTelemetryExtension.IsInitialized);
        }

        [Fact]
        public void AddMetricsServiceMeter_ShouldThrowIfCalledTwice()
        {
            var instrumentationName = "TestInstrumentation";

            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, instrumentationName);

            Assert.Throws<InvalidOperationException>(() => OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, instrumentationName));
        }

        [Fact]
        public void UseOpenTelemetry_ShouldThrowIfNotInitialized()
        {
            var appMock = new Mock<IApplicationBuilder>();
            var metricsTags = new List<MetricTagHolder<int>>();

            Assert.Throws<InvalidOperationException>(() => appMock.Object.UseOpenTelemetry(metricsTags));
        }

        [Fact]
        public void UseOpenTelemetry_ShouldAddMiddleware()
        {
            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, "TestInstrumentation");
            var appMock = new Mock<IApplicationBuilder>();
            var metricsTags = new List<MetricTagHolder<int>>();
            var nextMock = new Mock<RequestDelegate>();

            appMock.Setup(app => app.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Callback<Func<RequestDelegate, RequestDelegate>>(middleware =>
                {
                    var context = new DefaultHttpContext();
                    middleware(nextMock.Object).Invoke(context).Wait();
                });

            appMock.Object.UseOpenTelemetry(metricsTags);

            appMock.Verify(app => app.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
        }

        [Fact]
        public void UseOpenTelemetry_ShouldRegisterAndIncrementCounter()
        {
            OpenTelemetryExtension.AddMetricsServiceMeter(builderMock.Object, "TestInstrumentation");
            var appMock = new Mock<IApplicationBuilder>();
            var metricsTags = new List<MetricTagHolder<int>>
            {
                new("TestMetric", "Test Metric", "per request")
            };
            var context = new DefaultHttpContext();

            // Simulate a request to verify metrics processing
            var attributes = new List<KeyValuePair<string, object>>
            {
                new("TestLabel", 1)
            };
            var tagList = new TagList(new ReadOnlySpan<KeyValuePair<string, object?>>(attributes.ToArray()!));
            context.Items["TestMetric"] = new MetricTagHolder<int>("TestMetric", "Test Metric", "per request", tagList, 1);

            appMock.Setup(app => app.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Callback<Func<RequestDelegate, RequestDelegate>>(middleware =>
                {
                    middleware(async _ =>
                    {
                        // Mocked next delegate
                    }).Invoke(context).Wait();
                });

            // System Under Test execution
            appMock.Object.UseOpenTelemetry(metricsTags);

            // Validate
            Assert.True(OpenTelemetryExtension.registeredMetricCounters.ContainsKey("TestMetric"));
        }
    }
}
