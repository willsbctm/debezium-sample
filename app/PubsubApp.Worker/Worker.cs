using Google.Cloud.PubSub.V1;

namespace PubsubApp.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly SubscriberClientBuilder _subscriber;
    private const string _project = "test-project";
    private const string _subscription = "meu-topico-sub";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _subscriber = new SubscriberClientBuilder
        {
            SubscriptionName = new SubscriptionName(_project, _subscription),
            EmulatorDetection = Google.Api.Gax.EmulatorDetection.EmulatorOrProduction
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
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
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
