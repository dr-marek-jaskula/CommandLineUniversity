namespace CommandLiner.Common.Utilities;

public sealed class Disposable(Action dispose) : IDisposable
{
    private readonly Action _dispose = dispose;

    public static IDisposable Null { get; } = Create(() => {});

    public static IDisposable Create(Action dispose) => new Disposable(dispose);

    public void Dispose() => _dispose();
}