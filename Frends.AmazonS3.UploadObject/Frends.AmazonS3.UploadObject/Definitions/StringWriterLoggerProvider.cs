using System.IO;
using Microsoft.Extensions.Logging;

namespace Frends.AmazonS3.UploadObject.Definitions;

internal class StringWriterLoggerProvider : ILoggerProvider
{
    private readonly TextWriter _writer;

    public StringWriterLoggerProvider(TextWriter writer)
    {
        _writer = writer;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new StringWriterLogger(_writer, categoryName);
    }

    public void Dispose() { }
}