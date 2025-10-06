using StackShare.McpServer;
using StackShare.McpServer.Services;
using StackShare.McpServer.Tools;
using Serilog;
using MCPSharp;

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
