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
    /// <param name="errorObjects">The list of objects that encountered errors during delete operation</param>
    /// <returns>Result object with appropriate success status</returns>
    public static Result Handle(Exception exception, bool throwOnFailure, string errorMessage, List<SingleResultObject> deletedObjects, List<SingleResultObject> errorObjects = null)
    {
        if (throwOnFailure)
        {
            throw new Exception($"{errorMessage}\n{exception.Message}");
        }

        var error = new Error
        {
            Message = $"{errorMessage}\n{exception.Message}",
            AdditionalInfo = errorObjects
        };

        return new Result(false, deletedObjects, error);
    }
}
