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
    /// as the end result (bucket not existing) is the same as the intended outcome.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Contains error information when the operation fails or encounters issues.
    /// This property will be null when the operation succeeds without any warnings or errors.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Result class.
    /// </summary>
    /// <param name="success">Indicates whether the operation was successful.</param>
    /// <param name="error">Optional error information if the operation failed or encountered issues.</param>
    internal Result(bool success, Error error = null)
    {
        Success = success;
        Error = error;
    }
}

/// <summary>
/// Represents error information that occurred during the DeleteBucket task execution.
/// Provides detailed error messages and additional context information.
/// </summary>
public class Error
{
    /// <summary>
    /// The primary error message describing what went wrong during the operation.
    /// This message is suitable for display to users or logging purposes.
    /// </summary>
    /// <example>Failed to delete the bucket: Access denied</example>
    public string Message { get; private set; }

    /// <summary>
    /// Additional contextual information about the error.
    /// This can contain technical details, stack traces, or other relevant data
    /// that might be useful for debugging or detailed error analysis.
    /// </summary>
    /// <example>Bucket to be deleted, does not exist</example>
    public object AdditionalInfo { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Error class.
    /// </summary>
    /// <param name="message">The error message describing the issue.</param>
    /// <param name="additionalInfo">Optional additional context information about the error.</param>
    internal Error(string message, object additionalInfo = null)
    {
        Message = message;
        AdditionalInfo = additionalInfo;
    }
}
