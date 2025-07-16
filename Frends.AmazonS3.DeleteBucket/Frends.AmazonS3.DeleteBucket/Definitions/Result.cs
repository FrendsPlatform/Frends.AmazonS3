namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Result object returned by the DeleteBucket task containing operation status and error information.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the delete bucket operation completed successfully.
    /// Returns true if the bucket was deleted or if the bucket did not exist.
    /// Returns false only when an error occurred and ThrowErrorOnFailure is false.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Contains error information when the operation fails.
    /// This property is null when Success is true.
    /// Only populated when ThrowErrorOnFailure is false and an error occurs.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }

    internal Result(bool success)
    {
        Success = success;
        Error = null;
    }

    internal Result(bool success, Error error)
    {
        Success = success;
        Error = error;
    }
}
