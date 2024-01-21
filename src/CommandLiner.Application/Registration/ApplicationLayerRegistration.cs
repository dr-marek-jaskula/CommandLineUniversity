using Cocona;
using CommandLiner.Application.Commands.Copy;
using CommandLiner.Application.Commands.Health;
using CommandLiner.Application.Commands.Serialize;

namespace CommandLiner.Application.Registration;

public static class ApplicationLayerRegistration
{
    public static CoconaApp RegisterApplicationLayerCommands(this CoconaApp services)
    {
        services.AddCommands<CopyCommand>();
        services.AddCommands<SerializeCommand>();
        services.AddCommands<HealthCommand>();
        return services;
    }
}