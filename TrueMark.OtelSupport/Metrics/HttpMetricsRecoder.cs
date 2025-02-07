using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace TrueMark.OtelSupport.Metrics
{
    public class HttpMetricsRecorder
    {
        public const string TrueMarkHttpMetrics = "TrueMark.HttpMetrics";

        private readonly Histogram<double> requestDuration;

        public HttpMetricsRecorder()
        {
            var meter = new Meter(TrueMarkHttpMetrics);
            requestDuration = meter.CreateHistogram<double>(
                "http.server.request.duration",
                unit: "s",
                description: "Duration of HTTP server requests");
        }

        /// <summary>
        /// Records the duration of an HTTP server request along with various attributes.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request.</param>
        /// <param name="duration">The duration of the HTTP request in seconds.</param>
        /// <remarks>
        /// This method records the duration of an HTTP server request using OpenTelemetry metrics.
        /// The implementation is based on the OpenTelemetry specification for HTTP server request duration metrics:
        /// <see href="https://opentelemetry.io/docs/specs/semconv/http/http-metrics/#metric-httpserverrequestduration">OpenTelemetry HTTP Server Request Duration</see>.
        ///
        /// The following attributes are recorded:
        /// <list type="bullet">
        /// <item><description><c>http.request.method</c>: The HTTP method of the request.</description></item>
        /// <item><description><c>url.scheme</c>: The URL scheme of the request.</description></item>
        /// <item><description><c>http.response.status_code</c>: The HTTP status code of the response (if the response has started).</description></item>
        /// <item><description><c>http.route</c>: The route pattern matched by the request (if available).</description></item>
        /// <item><description><c>error.type</c>: The type of error (if the response status code indicates an error).</description></item>
        /// <item><description><c>network.protocol.name</c>: The name of the network protocol (assumed to be "http").</description></item>
        /// <item><description><c>network.protocol.version</c>: The version of the network protocol.</description></item>
        /// <item><description><c>server.address</c>: The server address.</description></item>
        /// <item><description><c>server.port</c>: The server port.</description></item>
        /// </list>
        /// </remarks>
        public void RecordDuration(HttpContext context, double duration)
        {
            var attributes = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("http.request.method", context.Request.Method),
                new KeyValuePair<string, object>("url.scheme", context.Request.Scheme),
            };

            // Conditionally Required: `http.response.status_code`
            if (context.Response.HasStarted)
            {
                attributes.Add(new KeyValuePair<string, object>("http.response.status_code", context.Response.StatusCode));
            }

            // Conditionally Required: `http.route`
            var route = HttpMetricsUtility.GetHttpRoute(context);
            if (!string.IsNullOrEmpty(route))
            {
                attributes.Add(new KeyValuePair<string, object>("http.route", route));
            }

            // Conditionally Required: `error.type` (Only if request failed)
            if (context.Response.StatusCode >= 400) // Capture errors (4xx, 5xx)
            {
                attributes.Add(new KeyValuePair<string, object>("error.type", $"HTTP {context.Response.StatusCode}"));
            }

            // Conditionally Required: `network.protocol.name` and `network.protocol.version`
            if (!string.IsNullOrEmpty(context.Request.Protocol))
            {
                attributes.Add(new KeyValuePair<string, object>("network.protocol.version", context.Request.Protocol.ToUpper().Replace("HTTP/", "")));
                attributes.Add(new KeyValuePair<string, object>("network.protocol.name", "http")); // Assume HTTP-based
            }

            // Optional: `server.address` & `server.port`
            if (!string.IsNullOrEmpty(context.Request.Host.Value))
            {
                attributes.Add(new KeyValuePair<string, object>("server.address", context.Request.Host.Host));
                attributes.Add(new KeyValuePair<string, object>("server.port", context.Request.Host.Port ?? (context.Request.IsHttps ? 443 : 80)));
            }

            // Record OpenTelemetry Metric
            requestDuration.Record(duration, attributes.ToArray());
        }
    }

    public static class HttpMetricsMiddleware
    {
        public static void UseHttpMetrics(this IApplicationBuilder app, HttpMetricsRecorder recorder)
        {
            app.Use(
                async (context, next) =>
                {
                    var startTime = DateTime.UtcNow;

                    await next();

                    var duration = (DateTime.UtcNow - startTime).TotalSeconds;
                    recorder.RecordDuration(context, duration);
                });
        }
    }
}
