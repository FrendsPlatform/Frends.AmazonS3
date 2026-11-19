namespace Frends.AmazonS3.ListObjectVersions.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to list versions.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>object { dynamic AdditionalInfo }</example>
    public dynamic AdditionalInfo { get; set; }
}
