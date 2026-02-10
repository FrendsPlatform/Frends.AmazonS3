using System.Collections.Generic;

namespace Frends.AmazonS3.ListObjectVersions.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; } = true;

    /// <summary>
    /// List of objects retrieved from the S3 bucket. Empty list if operation failed.
    /// </summary>
    /// <example>[{}, {}]</example>
    public List<BucketObjectVersions> Objects { get; set; } = [];

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
