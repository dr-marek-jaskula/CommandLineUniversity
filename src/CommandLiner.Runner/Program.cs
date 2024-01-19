using Cocona;
using CommandLiner.Application.Registration;
using Serilog;
using static CommandLiner.Runner.Utilities.LoggerUtilities;

Log.Logger = CreateSerilogLogger();

try
{
    Log.Information("Staring the host");

    var builder = CoconaApp.CreateBuilder();

    builder.ConfigureSerilog();

    builder.Services
        .RegisterInfrastructureLayerServices();

    builder
        .Build()
        .RegisterApplicationLayerCommands()
        .Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.Information("Ending the host");
    Log.CloseAndFlush();
}

return 0;

sealed partial class Program { }