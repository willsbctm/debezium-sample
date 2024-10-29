using Dapper;
using Npgsql;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/outbox", async (IConfiguration configuration, ILogger<Program> logger) =>
{
    logger.LogInformation("Getting outbox item");
    var connectionString = configuration.GetConnectionString("Sample");

    await using var connection = new NpgsqlConnection(connectionString);
    string sql = "SELECT id, data, type FROM outbox";
    var result = await connection.QueryAsync<Outbox>(sql);

    return Results.Ok(result);
})
.WithName("GetOutbox")
.WithOpenApi();

app.MapPost("/outbox", async (IConfiguration configuration, ILogger<Program> logger) =>
{
    logger.LogInformation("Creating outbox itens");
    var connectionString = configuration.GetConnectionString("Sample");
    var type = configuration.GetValue<string>("Type");

    await using var connection = new NpgsqlConnection(connectionString);

    var outbox = new Outbox
    {
        Id = Guid.NewGuid(),
        Data = JsonSerializer.Serialize(new { 
            Date = DateTime.Now
        }),
        Type = type
    };
    var sql = "INSERT INTO outbox (id, data, type) VALUES (@Id, CAST(@Data AS jsonb), @Type)";
    await connection.ExecuteAsync(sql, outbox);

    return Results.Created("outbox", outbox);
})
.WithName("PostOutbox")
.WithOpenApi();

app.MapDelete("/outbox", async (IConfiguration configuration, ILogger<Program> logger) =>
{
    logger.LogInformation("Deleting outbox itens");
    var connectionString = configuration.GetConnectionString("Sample");

    await using var connection = new NpgsqlConnection(connectionString);

    var sql = "DELETE FROM outbox";
    await connection.ExecuteAsync(sql);

    return Results.NoContent();
})
.WithName("DeleteOutbox")
.WithOpenApi();

app.Run();

public class Outbox
{
    public Guid Id { get; set; }
    public string Data { get; set; }
    public string Type { get; set; }
}

