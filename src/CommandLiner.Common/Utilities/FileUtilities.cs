namespace CommandLiner.Common.Utilities;

public static class FileUtilities
{
    public static bool FullPathNotContains(this FileInfo fileInfo, string pathSegment)
    {
        return fileInfo.FullName.Contains(pathSegment) is false;
    }

    public static void CopyTo(this FileInfo fileToCopy, DirectoryInfo destinationDirectoryInfo, bool overwrite = true)
    {
        string fileToCopyName = Path.GetFileName(fileToCopy.Name);
        string destinationLocation = Path.Combine(destinationDirectoryInfo.FullName, fileToCopyName);
        File.Copy(fileToCopy.FullName, destinationLocation, overwrite);
    }
}