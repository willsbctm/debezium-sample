using PubsubApp.Worker;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var topicName = builder.Configuration.GetValue<string>("TopicName");
var pubsubApi = builder.Configuration.GetValue<string>("PubsubApiUrl");
try
{
    using var client = new HttpClient();
    await client.PostAsync($"{pubsubApi}/topics", new StringContent(JsonSerializer.Serialize(new {
        name= topicName
    })));
}
catch(Exception ex)
{
    Console.WriteLine($"Erro creating topic {ex}");
}
try
{
    using var client = new HttpClient();
    await client.PostAsync($"{pubsubApi}/topics/{topicName}/subscriptions", new StringContent(JsonSerializer.Serialize(new {
        name= $"{topicName}-sub"
    })));
}
catch(Exception ex)
{
    Console.WriteLine($"Erro creating subscription {ex}");
}

var host = builder.Build();
host.Run();
