using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <param name="initialException">The exception to handle</param>
    /// <param name="throwOnFailure">Whether to throw the exception or return a failed result</param>
    /// <param name="errorMessage">Custom error message to use</param>
    /// <param name="deletedObjects">The list of successfully deleted objects</param>
    /// <param name="errorObjects">The list of objects that encountered errors during delete operation</param>
    /// <param name="errorExceptions">The list of exceptions that encountered during delete operation</param>
    /// <returns>Result object with appropriate success status</returns>
    public static Result Handle(Exception initialException, bool throwOnFailure, string errorMessage, List<SingleResultObject> deletedObjects, List<SingleResultObject> errorObjects = null, List<Exception> errorExceptions = null)
    {
        var combinedMessage = new StringBuilder()
            .AppendLine(errorMessage)
            .AppendLine(initialException.Message);

        if (errorObjects?.Count > 0 && errorExceptions?.Count > 0)
        {
            combinedMessage.AppendLine("Failed items:");
            foreach (var (obj, ex) in errorObjects.Zip(errorExceptions, (o, e) => (o, e)))
            {
                combinedMessage.AppendLine($"- {obj.Key} (Bucket: {obj.BucketName}): {ex.Message}");
            }
        }

        if (throwOnFailure)
        {
            if (initialException is AmazonS3Exception s3Ex)
            {
                throw new AmazonS3Exception(combinedMessage.ToString().Trim(), s3Ex.InnerException, s3Ex.ErrorType, s3Ex.ErrorCode, s3Ex.RequestId, s3Ex.StatusCode);
            }
            throw new Exception(combinedMessage.ToString().Trim(),
                errorExceptions?.Count == 1
                    ? errorExceptions[0]
                    : errorExceptions?.Count > 1
                        ? new AggregateException(errorExceptions)
                        : initialException);
        }

        return new Result(
            success: false,
            deletedObjects: deletedObjects,
            error: new Error
            {
                Message = combinedMessage.ToString().Trim(),
                AdditionalInfo = errorObjects
            });
    }
}
