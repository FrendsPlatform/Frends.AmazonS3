using Frends.AmazonS3.DeleteBucket.Definitions;
using System;

namespace Frends.AmazonS3.DeleteBucket.Helpers;

/// <summary>
/// Helper class for standardized error handling across the DeleteBucket task.
/// </summary>
public static class ErrorHandler
{
    /// <summary>
    /// Handles exceptions by either throwing a new exception or returning a Result with error information,
    /// based on the throwOnFailure parameter.
    /// </summary>
    /// <param name="exception">The original exception that occurred during the operation.</param>
    /// <param name="throwOnFailure">If true, throws a new exception with the custom message. If false, returns a Result with error details.</param>
    /// <param name="errorMessage">Custom error message to prepend to the original exception message.</param>
    /// <returns>A Result object with Success=false and populated Error information when throwOnFailure is false.</returns>
    /// <exception cref="Exception">Thrown when throwOnFailure is true, containing the custom error message and original exception details.</exception>
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
