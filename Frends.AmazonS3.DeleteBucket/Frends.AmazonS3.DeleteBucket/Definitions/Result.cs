namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Represents the result of the DeleteBucket task execution.
/// Contains information about the success status and any error details.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the bucket deletion operation completed successfully.
    /// The operation is considered successful even if the bucket to be deleted does not exist,
    /// as the end result (bucket not existing) is the same as a successful deletion.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Contains error information when the operation fails or encounters issues.
    /// This will be null when the operation succeeds without any problems.
    /// When not null, it provides details about what went wrong during execution.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Result class.
    /// </summary>
    /// <param name="success">Whether the operation was successful.</param>
    /// <param name="error">Error information if the operation failed.</param>
    internal Result(bool success, Error error = null)
    {
        Success = success;
        Error = error;
    }
}

/// <summary>
/// Represents error information when a DeleteBucket operation fails or encounters issues.
/// Provides both a user-friendly message and additional technical details.
/// </summary>
public class Error
{
    /// <summary>
    /// User-friendly error message describing what went wrong during the operation.
    /// This message can be customized through the Options.ErrorMessageOnFailure property.
    /// </summary>
    /// <example>Failed to delete the bucket</example>
    public string Message { get; private set; }

    /// <summary>
    /// Additional technical information about the error for debugging purposes.
    /// This can contain exception details, AWS error codes, or other contextual information
    /// that might help in troubleshooting the issue.
    /// </summary>
    /// <example>Bucket to be deleted, does not exist.</example>
    public dynamic AdditionalInfo { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Error class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="additionalInfo">Additional error context information.</param>
    internal Error(string message, dynamic additionalInfo = null)
    {
        Message = message;
        AdditionalInfo = additionalInfo;
    }
}
