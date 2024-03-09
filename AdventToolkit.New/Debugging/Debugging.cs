using System.Diagnostics;

namespace AdventToolkit.New.Debugging;

/// <summary>
/// Utilities for debugging.
/// </summary>
public static class Debugging
{
    private static bool _enableLogs;
    private static TraceListener? _logger;
    
    /// <summary>
    /// Enable printing of debug logs to the console.
    /// </summary>
    /// <param name="enable"></param>
    [Conditional("DEBUG")]
    public static void EnableLogs(bool enable = true)
    {
        if (_enableLogs == enable) return;
        
        _enableLogs = enable;
        if (_enableLogs)
        {
            Trace.Listeners.Add(_logger ??= new ConsoleTraceListener());
        }
        else
        {
            Trace.Listeners.Remove(_logger);
        }
    }
}