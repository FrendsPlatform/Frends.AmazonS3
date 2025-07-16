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
