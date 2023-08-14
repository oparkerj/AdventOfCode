namespace AdventToolkit.New.Util;

/// <summary>
/// Provides a means to temporarily capture console output.
/// </summary>
public sealed class ConsoleCapture : IDisposable
{
    private readonly StringWriter _writer = new();

    /// <summary>
    /// Get text that has been captured so far.
    /// </summary>
    public string Text => _writer.ToString();

    /// <summary>
    /// Begin console capture. Stores the current console output and replaces it with
    /// this console capture. When the returned object is disposed, console output
    /// is returned to the stored value.
    /// </summary>
    /// <returns>Disposable object to stop the capture.</returns>
    public IDisposable Start()
    {
        var restore = new CaptureRestore(this, Console.Out);
        Console.SetOut(_writer);
        return restore;
    }

    public void Dispose() => _writer.Dispose();

    /// <summary>
    /// Store console writer to restore later.
    /// Keeps a reference to the console capture so that it has a lifetime at least
    /// as long as the capture.
    /// </summary>
    private sealed class CaptureRestore : IDisposable
    {
        private readonly ConsoleCapture _source;
        private readonly TextWriter _oldOutput;

        public CaptureRestore(ConsoleCapture source, TextWriter oldOutput)
        {
            _source = source;
            _oldOutput = oldOutput;
        }

        public void Dispose()
        {
            Console.SetOut(_oldOutput);
        }
    }
}