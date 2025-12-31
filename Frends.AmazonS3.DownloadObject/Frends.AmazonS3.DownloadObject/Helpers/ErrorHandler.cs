using System;
using System.Collections.Generic;
using Frends.AmazonS3.DownloadObject.Definitions;

namespace Frends.AmazonS3.DownloadObject.Helpers;

/// <summary>
/// Static class for handling errors in the AmazonS3 DownloadObject task.
/// </summary>
public static class ErrorHandler
{
    /// <summary>
    /// Handles exceptions based on the provided options.
    /// </summary>
    /// <param name="exception">The exception to handle</param>
    /// <param name="options">Options containing error handling configuration</param>
    /// <param name="result">The current result list</param>
    /// <returns>Result object with error information</returns>
    public static Result Handle(Exception exception, Options options, List<SingleResultObject> result)
    {
        if (options.ThrowErrorOnFailure)
        {
            var errorMessage = !string.IsNullOrWhiteSpace(options.ErrorMessageOnFailure)
                ? options.ErrorMessageOnFailure
                : exception.Message;
            throw new Exception(errorMessage);
        }

        var error = new Error(exception.Message, new { exception.StackTrace, InnerException = exception.InnerException?.Message });
        return new Result(false, result, error);
    }
}
