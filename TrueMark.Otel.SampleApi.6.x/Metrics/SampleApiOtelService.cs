namespace TrueMark.Otel.SampleApi._6.x.Metrics;

public interface SampleApiOtelService
{
    void LogTestProcessedRequest();
    void LogTestSuccessfulRequest();
    void LogTestFailedRequest();
}