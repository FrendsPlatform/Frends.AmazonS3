using Frends.AmazonS3.DeleteBucket.Definitions;
using System;

namespace Frends.AmazonS3.DeleteBucket.Helpers;

/// <summary>
/// Helper class for error handling.
/// </summary>
public static class ErrorHandler
{
    /// <summary>
    /// Handle exceptions and return appropriate result or throw exception based on options.
    /// </summary>
    /// <param name="exception">The exception that occurred</param>
    /// <param name="throwOnFailure">Whether to throw exception on failure</param>
    /// <param name="errorMessage">Custom error message</param>
    /// <returns>Result with error information if not throwing</returns>
    public static Result Handle(Exception exception, bool throwOnFailure, string errorMessage)
    {
        if (throwOnFailure)
        {
            throw new Exception($"{errorMessage}\n{exception.Message}");
        }

        return new Result(false, new Error
        {
            Message = $"{errorMessage}\n{exception.Message}",
            AdditionalInfo = exception,
        });
    }
}
