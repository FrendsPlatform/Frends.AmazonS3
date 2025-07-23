using Frends.AmazonS3.UploadObject.Definitions;
using System;

namespace Frends.AmazonS3.UploadObject.Helpers;

/// <summary>
/// Static class for handling errors in a consistent way.
/// </summary>
public static class ErrorHandler
{
    /// <summary>
    /// Handle exceptions and return appropriate Result based on options.
    /// </summary>
    /// <param name="exception">The exception to handle</param>
    /// <param name="options">Options that determine how to handle the error</param>
    /// <returns>Result object with error information</returns>
    public static Result Handle(Exception exception, Options options)
    {
        if (options.ThrowErrorOnFailure)
        {
            if (string.IsNullOrEmpty(options.ErrorMessageOnFailure))
                throw new Exception(exception.Message, exception);
            throw new Exception(options.ErrorMessageOnFailure, exception);
        }
        var errorMessage = !string.IsNullOrEmpty(options.ErrorMessageOnFailure)
            ? $"{options.ErrorMessageOnFailure}: {exception.Message}"
            : exception.Message;
        return new Result
        {
            Success = false,
            Objects = null,
            Error = new Error
            {
                Message = errorMessage,
                AdditionalInfo = new
                {
                    Exception = exception,
                },
            },
        };
    }
}
