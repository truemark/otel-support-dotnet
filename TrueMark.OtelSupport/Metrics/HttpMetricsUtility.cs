using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace TrueMark.OtelSupport.Metrics
{
    /// <summary>
    /// Provides utility methods for HTTP metrics recording.
    /// </summary>
    public static class HttpMetricsUtility
    {
        /// <summary>
        /// Gets the HTTP route for the given <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request.</param>
        /// <returns>The route pattern matched by the request, or the request path if no route is matched.</returns>
        public static string GetHttpRoute(HttpContext context)
        {
            var endpointFeature = context.Features.Get<IEndpointFeature>();
            var endpoint = endpointFeature?.Endpoint;
            return endpoint?.DisplayName ?? context.Request.Path.Value;
        }
    }
}