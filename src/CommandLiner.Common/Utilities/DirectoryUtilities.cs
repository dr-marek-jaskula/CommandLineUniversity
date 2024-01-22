namespace CommandLiner.Common.Utilities;

public static class DirectoryUtilities
{
    public static DirectoryInfo CreateDirectoryInTemp()
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
        return Directory.CreateDirectory(directoryPath);
    }

    public static void ForceDelete(this DirectoryInfo directoryInfo)
    {
        if (directoryInfo.Exists is false)
        {
            return;
        }

        var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            file.Attributes = FileAttributes.Normal;
        }

        directoryInfo.Delete(true);
    }
}