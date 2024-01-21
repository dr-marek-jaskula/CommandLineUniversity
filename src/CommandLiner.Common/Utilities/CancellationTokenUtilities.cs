namespace CommandLiner.Common.Utilities;

public static class CancellationTokenUtilities
{
    public static void ThrowIfCancellationRequested(this CancellationToken cancellationToken, string message)
    {
        if (cancellationToken.IsCancellationRequested is false)
        {
            return;
        }

        throw new OperationCanceledException(message, cancellationToken);
    }
}