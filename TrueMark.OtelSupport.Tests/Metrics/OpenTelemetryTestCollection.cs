namespace TrueMark.OtelSupport.Tests.Metrics
{
    /// <summary>
    /// Test collection definition to ensure tests that share static OpenTelemetry state
    /// run sequentially rather than in parallel, preventing state collision.
    /// </summary>
    [CollectionDefinition("OpenTelemetry")]
    public class OpenTelemetryTestCollection
    {
        // This class is never instantiated. It exists only to define the collection.
    }
}
