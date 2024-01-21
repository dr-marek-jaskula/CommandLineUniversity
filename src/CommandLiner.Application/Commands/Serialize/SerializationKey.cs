using CommandLiner.Application.Enums;
using CommandLiner.Common.Cache;

namespace CommandLiner.Application.Commands.Serialize;

public sealed record class SerializationKey(FileType SourceType, FileType DestinationType) : Key;