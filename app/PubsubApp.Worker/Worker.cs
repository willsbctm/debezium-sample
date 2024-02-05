using Google.Cloud.PubSub.V1;

namespace PubsubApp.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly SubscriberClientBuilder _subscriber;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        var project = configuration.GetValue<string>("GcpProject");
        var subscription = $"{configuration.GetValue<string>("TopicName")}-sub";

        _logger = logger;
        _subscriber = new SubscriberClientBuilder
        {
            SubscriptionName = new SubscriptionName(project, subscription),
            EmulatorDetection = Google.Api.Gax.EmulatorDetection.EmulatorOrProduction
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            var subscriber = await _subscriber.BuildAsync(stoppingToken);
            await subscriber.StartAsync((message, token) => {
                try
                {
                    _logger.LogInformation($"Id {message.MessageId}");
                    var decodedMessage = message.Data.ToStringUtf8();

                    _logger.LogInformation($"Data {decodedMessage}");
                    return Task.FromResult(SubscriberClient.Reply.Ack);

                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "error processing message");
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }
            });
        }
    }
}
