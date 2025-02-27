using ProductApi.Services;

namespace ProductApi.Middleware
{
    public class ApiCallCount
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiCallCount> _logger;
        private readonly CloudWatchMetricsService _metricsService;
        private static int _requestCount = 0;

        public ApiCallCount(RequestDelegate next, ILogger<ApiCallCount> logger, CloudWatchMetricsService metricsService)
        {
            _next = next;
            _logger = logger;
            _metricsService = metricsService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            _requestCount++;
            _logger.LogInformation("API call to {Path} count: {Count}", context.Request.Path, _requestCount);

            // Send metrikken til CloudWatch
            await _metricsService.SendApiCallCountMetricAsync(_requestCount);

            await _next(context);
        }
    }

}
