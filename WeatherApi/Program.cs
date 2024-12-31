using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Serilog;
using Serilog.Sinks.Fluentd;

var builder = WebApplication.CreateBuilder(args);

// lab 03
builder.Logging.ClearProviders();
builder.Host.UseSerilog((ctx, config) =>
{
    config.ReadFrom.Configuration(ctx.Configuration);
    
    config
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Fluentd(new FluentdSinkOptions("localhost", 24224))
        .WriteTo.Console();
});

var app = builder.Build();

// lab 03
app.UseSerilogRequestLogging();

// lab 04
app.UseMetricServer();
app.UseHttpMetrics();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", ([FromServices] ILogger<Program> logger) =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        
        
        logger.LogInformation("Hello from manual log. Current UTC time: {Time}", DateTime.UtcNow);
        
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}