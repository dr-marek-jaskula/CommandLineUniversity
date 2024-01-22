// Ignore Spelling: Deserialize deserializer serializer yaml

using CommandLiner.Common.Utilities;
using YamlDotNet.Serialization;

namespace CommandLiner.Common.Serializers;

public static class YamlSerializer
{
    private static readonly IDeserializer _deserializer;
    private static readonly ISerializer _serializer;

    static YamlSerializer()
    {
        _deserializer = new DeserializerBuilder()
            .Build();
        _serializer = new SerializerBuilder()
            .Build();
    }

    public static T Deserialize<T>(string yaml)
    {
        using var stream = yaml.ToStream();
        using var reader = new StreamReader(stream);
        return _deserializer.Deserialize<T>(reader);
    }

    public static T Deserialize<T>(TextReader reader)
    {
        return _deserializer.Deserialize<T>(reader);
    }

    public static T Deserialize<T>(FileInfo fileInfo)
    {
        using var reader = new StreamReader(fileInfo.FullName);
        return _deserializer.Deserialize<T>(reader.ReadToEnd())
            ?? throw new FormatException($"Unable to deserialize '{fileInfo.FullName}' to '{nameof(T)}'.");
    }

    public static object Deserialize(FileInfo fileInfo)
    {
        using var reader = new StreamReader(fileInfo.FullName);
        return _deserializer.Deserialize(reader.ReadToEnd())
            ?? throw new FormatException($"Unable to deserialize '{fileInfo.FullName}'.");
    }

    public static string Serialize<T>(T input)
    {
        return _serializer.Serialize(input);
    }
}