namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Result object containing the outcome of the DeleteBucket operation.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the bucket deletion operation completed successfully.
    /// Returns true if the bucket was deleted or if the bucket did not exist.
    /// Returns false only when ThrowErrorOnFailure is set to false and an error occurred.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Contains error information when the operation fails and ThrowErrorOnFailure is set to false.
    /// Will be null when the operation succeeds or when ThrowErrorOnFailure is true (exceptions are thrown instead).
    /// </summary>
    /// <example>null</example>
    public Error? Error { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Result class.
    /// </summary>
    /// <param name="success">Whether the operation was successful.</param>
    /// <param name="error">Error information if the operation failed.</param>
    internal Result(bool success, Error? error = null)
    {
        Success = success;
        Error = error;
    }
}

/// <summary>
/// Contains detailed error information when a DeleteBucket operation fails.
/// </summary>
public class Error
{
    /// <summary>
    /// Human-readable error message describing what went wrong during the bucket deletion operation.
    /// </summary>
    /// <example>Bucket to be deleted, does not exist.</example>
    public string Message { get; private set; }

    /// <summary>
    /// Additional context information about the error, such as the original exception object.
    /// This can contain technical details useful for debugging.
    /// </summary>
    /// <example>null</example>
    public dynamic? AdditionalInfo { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Error class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="additionalInfo">Additional error context information.</param>
    internal Error(string message, dynamic? additionalInfo = null)
    {
        Message = message;
        AdditionalInfo = additionalInfo;
    }
}
