# TrueMark.OtelSupport

[![NuGet](https://img.shields.io/nuget/v/TrueMark.OtelSupport)](https://www.nuget.org/packages/TrueMark.OtelSupport/)
[![License](https://img.shields.io/github/license/truemark/otel-support-dotnet)](https://github.com/truemark/otel-support-dotnet/blob/main/TrueMark.OtelSupport/LICENSE)

## Overview

**TrueMark.OtelSupport** is a .NET library that provides OpenTelemetry support for instrumenting applications. It simplifies distributed tracing and metrics collection using OpenTelemetry standards.

## Installation

You can install this package via NuGet:

```sh
dotnet add package TrueMark.OtelSupport --version 1.0.0
```
Or via the NuGet Package Manager:
```sh
Install-Package TrueMark.OtelSupport -Version 1.0.0
```

## Compatibility

- .NET Standard 2.0 (Compatible with .NET Core 2.1+, .NET 6+, .NET 8+)
- OpenTelemetry API version 1.9.0

## Usage
This library is mainly targeting web based dotnet applications. It provides a middleware that can be used to automatically instrument incoming requests and outgoing responses. It also has support for custom metrics.
Below are the 2 major use cases that this library supports:

### 1. Adding default instrumentation
Given an instance `services` of `IServiceCollection`, you can enable OTEL with the below code in the `Program.cs` or `Startup.cs` file:
```csharp
var resourceBuilder = ResourceBuilder
                      .CreateDefault()
                      .AddTelemetrySdk()
                      .AddEnvironmentVariableDetector()
                      .AddService(<service-name-here>) // Including environment e.g. payment-api-dev
                      .AddDetector(new AWSECSResourceDetector());
services.AddOpenTelemetry()
        .WithTracing(
            tracing => tracing
                       .AddAspNetCoreInstrumentation()
                       .AddXRayTraceId()
                       .SetResourceBuilder(resourceBuilder)
                       .AddAWSInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddAspNetCoreInstrumentation()
                       .AddOtlpExporter())
        .WithMetrics(
            metrics => metrics
                       .SetResourceBuilder(resourceBuilder)
                       .AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddRuntimeInstrumentation()
                       .AddProcessInstrumentation()

                       // Only use "overrideHttpMetrics" value of true when using Minimal APIs
                       .AddMetricsServiceMeter(otelServiceName, true)

                       .AddOtlpExporter());
```

When using the `Minimal APIs` - default `http.server*` metrics don't emit by default, thus you can use the below code in the `Program.cs`/`Startup.cs` file to enable OTEL (where `app` is an instance of `WebApplication`):
```csharp
// Only add this line when using Minimal APIs in .NET
app.UseHttpMetrics(new HttpMetricsRecorder());
```


### 2. Adding custom metrics
To add custom metrics to your application, you need to do the following:

1) Extend the `IMetricsRegistry` and register your metrics with the `IMetricsRegistry` instance. Below is an example of how to add a custom metric:
```csharp
public class CustomMetricsRegistry : IMetricsRegistry
    {
        public static readonly MetricTagHolder<long> AllTransactionsMetric = new MetricTagHolder<long>("transaction_event", "All Transaction related events", "per request");
        public static readonly MetricTagHolder<double> PaidTransactionMetric = new MetricTagHolder<double>("payment_transaction_event", "Payment Transaction related events", "per request");
        public void LoadMetrics(IApplicationBuilder app)
        {
            var metricsTagsLongType = new List<MetricTagHolder<long>>
            {
                AllTransactionsMetric
            };
            app.UseOpenTelemetry(metricsTagsLongType);

            var metricsTagsDoubleType = new List<MetricTagHolder<double>>
            {
                PaidTransactionMetric
            };
            app.UseOpenTelemetry(metricsTagsDoubleType);
        }
    }
```
2) In the `Program.cs` or `Startup.cs` file, load the custom metrics with the `IMetricsRegistry` instance:
```csharp
// Add Otel Metrics Registry for custom metrics here
new CustomMetricsRegistry().LoadMetrics(app);
```
## License

This project is licensed under the **BSD 3-Clause License** - see the [LICENSE](LICENSE) file for details.

