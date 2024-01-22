using Cocona;
using Microsoft.Extensions.Logging;
using static CommandLiner.Common.Utilities.FileUtilities;

namespace CommandLiner.Application.Commands.Copy;

public sealed class CopyCommand(ILogger<CopyCommand> logger)
{
    private readonly ILogger<CopyCommand> _logger = logger;

    [Command("copy", Description = "Use to copy files from source directory to destination directory")]
    public void Handle
    (
        [Option("source", shortNames: ['s'], Description = "source path, relative to current folder.")]
        string source,
        [Option("destination", shortNames: ['d'], Description = "destination path, relative to current folder.")]
        string destination,
        [Option("fileNames", shortNames: ['f'], Description = "Files to copy. Can be provided with extension or without.")]
        string[] fileNames,
        [Option("searchSubdirectories", shortNames: ['t'], Description = "If true, than all subdirectories will be searched. Otherwise, only top directory will be searched.")]
        bool TopDirectoryOnly,
        [Option("searchPattern", shortNames: ['p'], Description = "Search pattern. Use to search only files with certain extension. For example '*.json'.")]
        string searchPattern = "*"
    )
    {
        var sourceDirectory = new DirectoryInfo(source);
        var destinationDirectory = new DirectoryInfo(destination);

        _logger.LogInformation("Source path: '{FullName}'", sourceDirectory.FullName);
        _logger.LogInformation("Destination path: '{FullName}'", destinationDirectory.FullName);

        try
        {
            destinationDirectory.Create();

            var searchOptions = TopDirectoryOnly
                ? SearchOption.TopDirectoryOnly
                : SearchOption.AllDirectories;

            var filesToCopy = sourceDirectory
                .GetFiles(searchPattern, searchOptions)
                .Where(file => fileNames.Contains(Path.GetFileNameWithoutExtension(file.Name)) 
                            || fileNames.Contains(Path.GetFileName(file.Name)))
                .ToArray();

            if (filesToCopy.Length is 0)
            {
                _logger.LogWarning("Files not found.");
                return;
            }

            foreach (var file in filesToCopy)
            {
                file.CopyTo(destinationDirectory);
                _logger.LogInformation("Copied {FullName} to {FullName} with override option", file.FullName, destinationDirectory.FullName);
            }
        }
        catch (Exception error)
        {
            _logger.LogError("An error was occurred {Message}", error.Message);
        }
    }
}