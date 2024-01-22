using CommandLiner.Application.Enums;
using CommandLiner.Common.Cache;

namespace CommandLiner.Application.Commands.Serialize;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class SerializationStrategyAttribute(FileType SourceType, FileType DestinationType) : CacheAttribute<SerializationKey>
{
    public FileType SourceType { get; } = SourceType;
    public FileType DestinationType { get; } = DestinationType;

    public override SerializationKey ToKey()
    {
        return new SerializationKey(SourceType, DestinationType);
    }
}