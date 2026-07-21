using System.IO;
using Microsoft.Extensions.Logging;

namespace Frends.AmazonS3.UploadObject.Definitions;

internal class StringWriterLoggerProvider(TextWriter writer) : ILoggerProvider
{
    /// <summary>Creates a new logger instance for the specified category.</summary>
    public ILogger CreateLogger(string categoryName)
    {
        return new StringWriterLogger(writer, categoryName);
    }

    /// <summary>Disposes the provider. The underlying TextWriter is not disposed here as it is managed by the caller.</summary>
    public void Dispose()
    {
        // Note: We should NOT dispose _writer here as it may be used elsewhere
        // The caller who created the TextWriter should be responsible for disposing it
    }
}