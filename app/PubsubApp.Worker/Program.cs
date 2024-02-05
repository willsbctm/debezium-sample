using PubsubApp.Worker;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var pubsubApi = "http://pubsub:8000/api";
try
{
    using var client = new HttpClient();
    await client.PostAsync($"{pubsubApi}/topics", new StringContent(JsonSerializer.Serialize(new {
        name= "meu-topico"
    })));
}
catch(Exception ex)
{
    Console.WriteLine($"Erro creating topic {ex}");
}
try
{
    using var client = new HttpClient();
    await client.PostAsync($"{pubsubApi}/topics/meu-topico/subscriptions", new StringContent(JsonSerializer.Serialize(new {
        name= "meu-topico-sub"
    })));
}
catch(Exception ex)
{
    Console.WriteLine($"Erro creating subscription {ex}");
}

var host = builder.Build();
host.Run();
