using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter(), 
        path: "logs/stackshare-.json",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog to the host
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(StackShare.Application.AssemblyReference).Assembly));

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("StackShare.API", "1.0.0"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter());

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Serilog request logging
app.UseSerilogRequestLogging();

app.MapControllers();

Log.Information("StackShare API starting up");

app.Run();


