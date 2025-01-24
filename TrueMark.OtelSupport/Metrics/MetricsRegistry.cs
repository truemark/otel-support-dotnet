using Microsoft.AspNetCore.Builder;

namespace TrueMark.OtelSupport.Metrics
{
    public interface IMetricsRegistry
    {
        void LoadMetrics(IApplicationBuilder app);
    }
}
