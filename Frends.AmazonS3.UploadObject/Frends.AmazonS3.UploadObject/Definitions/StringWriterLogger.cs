using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Frends.AmazonS3.UploadObject.Definitions;

internal class StringWriterLogger(TextWriter writer, string category) : ILogger
{
    private readonly object _syncRoot = new();

    /// <summary>Begins a logical operation scope.</summary>
    public IDisposable BeginScope<TState>(TState state) => null;

    /// <summary>Checks if the given log level is enabled.</summary>
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <summary>Writes a log entry to the underlying TextWriter.</summary>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);
        lock (_syncRoot)
        {
            writer.WriteLine($"[{logLevel}] {category}: {message}");
            if (exception != null)
                writer.WriteLine(exception);
        }
    }
}