using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Frends.AmazonS3.UploadObject.Definitions;

internal class StringWriterLogger(TextWriter writer, string category) : ILogger
{
    private readonly object _syncRoot = new();

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

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