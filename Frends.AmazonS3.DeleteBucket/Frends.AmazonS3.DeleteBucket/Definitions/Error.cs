namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Error information for task operations.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message describing what went wrong.
    /// </summary>
    /// <example>Failed to delete the bucket.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error, can contain any type of data.
    /// </summary>
    /// <example>Additional error details</example>
    public dynamic AdditionalInfo { get; set; }
}
