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
    /// <param name="ex">The original exception that occurred during the operation.</param>
    /// <param name="throwOnFailure">If true, throws a new exception with the custom message. If false, returns a Result with error details.</param>
    /// <param name="customErrorMessage">Custom error message to prepend to the original exception message.</param>
    /// <returns>object { bool Success, object Error { string Message, dynamic AdditionalInfo } } </returns>
    /// <exception cref="Exception">Thrown when throwOnFailure is true, containing the custom error message and original exception details.</exception>
    public static Result Handle(Exception ex, bool throwOnFailure, string customErrorMessage)
    {
        if (throwOnFailure)
        {
            if (string.IsNullOrEmpty(customErrorMessage))
                throw new Exception(ex.Message, ex);

            throw new Exception(customErrorMessage, ex);
        }

        var errorMessage = !string.IsNullOrEmpty(customErrorMessage)
            ? $"{customErrorMessage}: {ex.Message}"
            : ex.Message;

        return new Result
        {
            Success = false,
            Error = new Error
            {
                Message = errorMessage,
                AdditionalInfo = new
                {
                    Exception = ex,
                },
            },
        };
    }
}
