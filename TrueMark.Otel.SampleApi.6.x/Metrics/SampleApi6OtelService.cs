namespace TrueMark.Otel.SampleApi._6.x.Metrics;

public interface SampleApi6OtelService
{
    void LogTestProcessedRequest();
    void LogTestSuccessfulRequest();
    void LogTestFailedRequest();
}