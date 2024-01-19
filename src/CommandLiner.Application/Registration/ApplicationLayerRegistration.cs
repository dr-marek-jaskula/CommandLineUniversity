using Cocona;
using CommandLiner.Application.Commands;

namespace CommandLiner.Application.Registration;

public static class ApplicationLayerRegistration
{
    public static CoconaApp RegisterApplicationLayerCommands(this CoconaApp services)
    {
        services
            .AddCommands<CopyCommand>();

        return services;
    }
}