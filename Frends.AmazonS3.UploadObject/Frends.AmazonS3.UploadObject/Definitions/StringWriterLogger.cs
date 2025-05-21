using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Frends.AmazonS3.UploadObject.Definitions;

internal class StringWriterLogger : ILogger
{
    private readonly TextWriter _writer;
    private readonly string _category;

    public StringWriterLogger(TextWriter writer, string category)
    {
        _writer = writer;
        _category = category;
    }

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
        _writer.WriteLine($"[{logLevel}] {_category}: {message}");

        if (exception != null)
        {
            _writer.WriteLine(exception);
        }
    }
}