namespace TrueMark.Otel.SampleApi._8.x.Metrics;

public interface SampleApi8OtelService
{
    void LogTestProcessedRequest();
    void LogTestSuccessfulRequest();
    void LogTestFailedRequest();
}