using StackShare.McpServer;
using StackShare.McpServer.Services;
using StackShare.McpServer.Tools;
using Serilog;
using MCPSharp;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

// Add Serilog
builder.Services.AddSerilog();

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("StackShare.McpServer", "1.0.0")
            .AddAttributes(new Dictionary<string, object>
            {
                ["service.version"] = "1.0.0",
                ["service.environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                ["service.instance.id"] = Environment.MachineName,
                ["service.type"] = "mcp-server"
            }))
        .AddHttpClientInstrumentation(options =>
        {
            options.RecordException = true;
            options.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) =>
            {
                activity.SetTag("http.client.method", httpRequestMessage.Method.Method);
                activity.SetTag("http.client.url", httpRequestMessage.RequestUri?.ToString());
                activity.SetTag("mcp.client", "stackshare");
                
                // Propagate correlation ID if present
                if (httpRequestMessage.Headers.Contains("X-Correlation-ID"))
                {
                    var correlationId = httpRequestMessage.Headers.GetValues("X-Correlation-ID").FirstOrDefault();
                    activity.SetTag("correlation_id", correlationId);
                }
            };
        })
        .AddSource("StackShare.McpServer.Tools")
        .AddConsoleExporter());

// Configure HttpClient for StackShare API
builder.Services.AddHttpClient<IStackShareApiClient, StackShareApiClient>(client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("StackShareApi:BaseUrl") ?? "http://localhost:5000/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("User-Agent", "StackShare.McpServer/1.0.0");
});

// Register MCP Server as hosted service
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

try
{
    Log.Information("Iniciando StackShare MCP Server...");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Falha crítica ao iniciar o servidor");
}
finally
{
    Log.CloseAndFlush();
}
