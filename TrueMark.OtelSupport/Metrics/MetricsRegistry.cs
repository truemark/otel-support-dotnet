using Microsoft.AspNetCore.Builder;

namespace TrueMark.OtelSupport.Metrics
{
    /// <summary>
    /// Interface for loading and managing metrics in an application.
    /// </summary>
    public interface IMetricsRegistry
    {
        /// <summary>
        /// Loads and configures metrics for the application.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to configure metrics for.</param>
        void LoadMetrics(IApplicationBuilder app);
    }
}