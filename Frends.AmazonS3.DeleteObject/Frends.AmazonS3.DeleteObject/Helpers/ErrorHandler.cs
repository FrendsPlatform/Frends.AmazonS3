using System;
using System.Collections.Generic;
using Amazon.S3;
using Frends.AmazonS3.DeleteObject.Definitions;

namespace Frends.AmazonS3.DeleteObject.Helpers;

/// <summary>
/// Helper class for handling errors in Amazon S3 operations.
/// </summary>
public static class ErrorHandler
{
    /// <summary>
    /// Handles exceptions and returns appropriate Result object.
    /// </summary>
    /// <param name="exception">The exception to handle</param>
    /// <param name="throwOnFailure">Whether to throw the exception or return a failed result</param>
    /// <param name="errorMessage">Custom error message to use</param>
    /// <param name="deletedObjects">The list of successfully deleted objects</param>
    /// <returns>Result object with appropriate success status</returns>
    public static Result Handle(Exception exception, bool throwOnFailure, string errorMessage, List<SingleResultObject> deletedObjects)
    {
        var finalErrorMessage = string.IsNullOrEmpty(errorMessage)
            ? GetDefaultErrorMessage(exception)
            : errorMessage;

        if (throwOnFailure)
        {
            if (exception is AmazonS3Exception)
                throw new AmazonS3Exception(finalErrorMessage, exception);
            else
                throw new Exception(finalErrorMessage, exception);
        }

        var error = new Error
        {
            ErrorMessage = finalErrorMessage,
            AdditionalInfo = new List<SingleResultObject>() // Objects that failed will be added here by caller if needed
        };

        return new Result(false, deletedObjects, error);
    }

    private static string GetDefaultErrorMessage(Exception exception)
    {
        return exception switch
        {
            AmazonS3Exception => $"DeleteObject AmazonS3Exception: {exception.Message}",
            _ => $"DeleteObject Exception: {exception.Message}"
        };
    }
}
