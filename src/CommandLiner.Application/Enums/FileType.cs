using System.Security.AccessControl;

namespace CommandLiner.Application.Enums;

public enum FileType
{
    Json,
    Yaml
}

public static class FileTypeUtilities
{
    public static string GetExtensionWithDot(this FileType fileType)
    {
        return $".{fileType}".ToLower();
    }
}