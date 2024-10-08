using OpenTelemetry.Contrib.Extensions.AWSXRay.Resources;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TrueMark.Otel.Helper;
using TrueMark.Otel.SampleApi._6.x.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use specific ports
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8086); // HTTP port
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<SampleApi8OtelService, SampleApi8OtelServiceImpl>();

// Configure OpenTelemetry
ConfigureOpenTelemetry(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Add Metrics Registry here
new SampleApi8MetricsRegistry().LoadMetrics(app);

void ConfigureOpenTelemetry(IServiceCollection services)
{
    var otelServiceName = "Test2-Service";
    var resourceBuilder = ResourceBuilder
        .CreateDefault()
        .AddTelemetrySdk()
        .AddEnvironmentVariableDetector()
        .AddService(otelServiceName)
        .AddDetector(new AWSECSResourceDetector());

    services.AddOpenTelemetry()
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddXRayTraceId()
            .SetResourceBuilder(resourceBuilder)
            .AddAWSInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter()
            .AddConsoleExporter()) // Console exporter is not needed in production, it is used here for demonstration purposes on local machine
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .SetResourceBuilder(resourceBuilder)
            .AddMetricsServiceMeter(otelServiceName)
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddOtlpExporter()
            .AddConsoleExporter()); // Console exporter is not needed in production, it is used here for demonstration purposes on local machine
}