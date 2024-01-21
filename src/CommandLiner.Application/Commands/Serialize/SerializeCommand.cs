using Cocona;
using CommandLiner.Application.Enums;
using CommandLiner.Common.Factories;
using CommandLiner.Common.Serializers;
using CommandLiner.Common.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Reflection;

namespace CommandLiner.Application.Commands.Serialize;

public sealed class SerializeCommand(ILogger<SerializeCommand> logger)
{
    private static readonly ReadOnlyDictionary<SerializationKey, Func<FileInfo, string>> _strategyCache;

    static SerializeCommand()
    {
        var strategies = typeof(SerializeCommand)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(method => method.GetCustomAttribute<SerializationStrategyAttribute>() is not null)
            .Select(x => x.CreateDelegate<Func<FileInfo, string>>());

        _strategyCache = CacheFactory<SerializationKey, Func<FileInfo, string>>
            .CreateFor<SerializationStrategyAttribute>(strategies);
    }

    private readonly ILogger<SerializeCommand> _logger = logger;

    [Command("serialize", Description = "Use to serialize file from one format to another")]
    public void Handle
    (
        [Option('p', Description = "source path, relative to current folder.")]
        string sourcePath,
        [Option('n', Description = "source file name with type.")]
        string fileName,
        [Option('s', Description = "source file type.")]
        FileType sourceType,
        [Option('d', Description = "destination file type.")]
        FileType destinationType
    )
    {
        var sourceDirectory = new DirectoryInfo(sourcePath);

        _logger.LogInformation("Source path: '{FullName}'", sourceDirectory.FullName);

        try
        {
            var sourceTypeWithDotAsString = sourceType.GetExtensionWithDot();

            fileName = ValidateFileName(fileName, sourceTypeWithDotAsString);

            var fileToSerialize = sourceDirectory
                .GetFiles($"{fileName}{sourceTypeWithDotAsString}", SearchOption.TopDirectoryOnly)
                .Single();

            var strategyKey = new SerializationKey(sourceType, destinationType);

            if (_strategyCache.TryGetValue(strategyKey, out var @delegate) is false)
            {
                throw new ArgumentException($"Strategy for source type '{sourceType}' and destination type '{destinationType}' not supported.");
            }

            string serializedFileContent = @delegate(fileToSerialize);

            var destinationTypeWithDotAsString = destinationType.GetExtensionWithDot();
            var newFileName = fileToSerialize.FullName.Replace(sourceTypeWithDotAsString, destinationTypeWithDotAsString);
            using var fileStream = File.Create(newFileName);
            fileStream.Write(serializedFileContent.ToBytes());
        }
        catch (Exception error)
        {
            _logger.LogError("An error was occurred {Message}", error.Message);
        }
    }

    private static string ValidateFileName(string fileName, string sourceTypeWithDotAsString)
    {
        var indexOfExtension = fileName.IndexOf(sourceTypeWithDotAsString);

        if (indexOfExtension != -1)
        {
            fileName = fileName[..indexOfExtension];
        }

        return fileName;
    }

    [SerializationStrategy(FileType.Yaml, FileType.Json)]
    private static string FromYamlToJson(FileInfo fileToSerialize)
    {
        var deserializedFileContent = YamlSerializer.Deserialize(fileToSerialize);
        return JsonConvert.SerializeObject(deserializedFileContent);
    }

    [SerializationStrategy(FileType.Json, FileType.Yaml)]
    private static string FromJsonToYaml(FileInfo fileToSerialize)
    {
        using var reader = new StreamReader(fileToSerialize.FullName);
        var expConverter = new ExpandoObjectConverter();
        dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(reader.ReadToEnd(), expConverter)
            ?? throw new ArgumentException("Provided file is not a json file");
        return YamlSerializer.Serialize(deserializedObject);
    }
}