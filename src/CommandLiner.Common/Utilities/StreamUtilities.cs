using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace CommandLiner.Common.Utilities;

public static class StreamUtilities
{
    public const int Stream = 81920;
    public const int StreamReader = 1024;

    public static async Task CopyToAsync(this Stream source, Stream destination, bool autoFlush, CancellationToken cancellationToken)
    {
        using var buffer = MemoryPool<byte>.Shared.Rent(Stream);

        while (true)
        {
            var bytesRead = await source.ReadAsync(buffer.Memory, cancellationToken).ConfigureAwait(false);

            if (bytesRead <= 0)
            {
                break;
            }

            await destination.WriteAsync(buffer.Memory[..bytesRead], cancellationToken).ConfigureAwait(false);

            if (autoFlush)
            {
                await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public static async IAsyncEnumerable<string> ReadAllLinesAsync(this StreamReader reader, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var lineBuffer = new StringBuilder();
        using var buffer = MemoryPool<char>.Shared.Rent(StreamReader);

        var isLastCaretReturn = false;

        while (true)
        {
            var charsRead = await reader.ReadAsync(buffer.Memory, cancellationToken).ConfigureAwait(false);

            if (charsRead <= 0)
            {
                break;
            }

            for (var i = 0; i < charsRead; i++)
            {
                var character = buffer.Memory.Span[i];

                if (isLastCaretReturn && character == '\n')
                {
                    isLastCaretReturn = false;
                    continue;
                }

                if (character is '\n' or '\r')
                {
                    yield return lineBuffer.ToString();
                    lineBuffer.Clear();
                }
                else
                {
                    lineBuffer.Append(character);
                }

                isLastCaretReturn = character == '\r';
            }
        }

        if (lineBuffer.Length > 0)
        {
            yield return lineBuffer.ToString();
        }
    }
}