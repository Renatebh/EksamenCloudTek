using Amazon.CloudWatch.Model;
using Amazon.CloudWatch;

namespace ProductApi.Services
{
    public class CloudWatchMetricsService
    {
        private readonly IAmazonCloudWatch _cloudWatchClient;
        private readonly ILogger<CloudWatchMetricsService> _logger;

        public CloudWatchMetricsService(IAmazonCloudWatch cloudWatchClient, ILogger<CloudWatchMetricsService> logger)
        {
            _cloudWatchClient = cloudWatchClient;
            _logger = logger;
        }

        public async Task SendApiCallCountMetricAsync(int count)
        {
            var metricDatum = new MetricDatum
            {
                MetricName = "ApiCallCount",
                Unit = StandardUnit.Count,
                Value = count,
                Timestamp = DateTime.UtcNow
            };

            var request = new PutMetricDataRequest
            {
                Namespace = "ProductApi",
                MetricData = new List<MetricDatum> { metricDatum }
            };

            try
            {
                await _cloudWatchClient.PutMetricDataAsync(request);
                _logger.LogInformation("Successfully sent ApiCallCount metric to CloudWatch.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending ApiCallCount metric to CloudWatch: {ex.Message}");
            }
        }
    }

}
