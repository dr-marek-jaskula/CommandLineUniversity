namespace CommandLiner.Common.Utilities;

public static class FileStreamUtilities
{
    public static void WriteAndClose(this FileStream fileStream, string content)
    {
        fileStream.Write(content.ToBytes().AsSpan());
        fileStream.Close();
    }
}