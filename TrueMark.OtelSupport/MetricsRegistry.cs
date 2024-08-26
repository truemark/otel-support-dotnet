using Microsoft.AspNetCore.Builder;

namespace TrueMark.Otel.Helper
{
    public interface IMetricsRegistry
    {
        void LoadMetrics(IApplicationBuilder app);
    }
}
