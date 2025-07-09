namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// The operation is complete without errors.
    /// The operation is considered successful if the bucket to be deleted does not exist.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Error information when the operation fails.
    /// </summary>
    public Error Error { get; private set; }

    internal Result(bool success, Error error = null)
    {
        Success = success;
        Error = error;
    }
}

/// <summary>
/// Error information structure.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message description.
    /// </summary>
    /// <example>Failed to delete the bucket</example>
    public string Message { get; private set; }

    /// <summary>
    /// Additional error context information.
    /// </summary>
    public dynamic AdditionalInfo { get; private set; }

    internal Error(string message, dynamic additionalInfo = null)
    {
        Message = message;
        AdditionalInfo = additionalInfo;
    }
}
