using Cocona.Builder;
using Serilog;
using static Serilog.Events.LogEventLevel;

namespace CommandLiner.Runner.Utilities;

public static class LoggerUtilities
{
    private const string Microsoft = nameof(Microsoft);

    public static Serilog.ILogger CreateSerilogLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Override(nameof(Microsoft), Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
    }

    public static void ConfigureSerilog(this CoconaAppBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
    }
}