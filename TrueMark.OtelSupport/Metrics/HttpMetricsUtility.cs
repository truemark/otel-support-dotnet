using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace TrueMark.OtelSupport.Metrics
{
    public static class HttpMetricsUtility
    {
        public static string GetHttpRoute(HttpContext context)
        {
            var endpointFeature = context.Features.Get<IEndpointFeature>();
            var endpoint = endpointFeature?.Endpoint;
            return endpoint?.DisplayName ?? context.Request.Path.Value;
        }

    }
}
