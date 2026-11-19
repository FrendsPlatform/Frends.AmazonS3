namespace Frends.AmazonS3.CreateBucket.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to create bucket.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>object { dynamic AdditionalInfo }</example>
    public dynamic AdditionalInfo { get; set; }
}
