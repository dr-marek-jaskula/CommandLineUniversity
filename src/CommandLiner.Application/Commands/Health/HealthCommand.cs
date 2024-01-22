using Cocona;
using Microsoft.Extensions.Logging;

namespace CommandLiner.Application.Commands.Health;

public sealed class HealthCommand(ILogger<HealthCommand> logger)
{
    private readonly ILogger<HealthCommand> _logger = logger;

    static HealthCommand()
    {
        Console.WriteLine("Host is healthy static.");
    }

    [Command("health", Description = "Check CommandLiner health.")]
    public void Handle()
    {
        _logger.LogInformation("Host is healthy.");
    }
}