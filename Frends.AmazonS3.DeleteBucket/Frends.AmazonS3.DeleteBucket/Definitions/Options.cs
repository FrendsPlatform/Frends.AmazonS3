using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Options for configuring the behavior of the DeleteBucket task.
/// </summary>
public class Options
{
    /// <summary>
    /// Determines whether to throw an exception when the bucket deletion fails.
    /// When set to true, exceptions will be thrown on failure.
    /// When set to false, errors will be returned in the Result object instead.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Custom error message to use when ThrowErrorOnFailure is false and an error occurs.
    /// If not provided or null, a default error message will be used based on the type of error.
    /// This allows for more user-friendly error messages in the application.
    /// </summary>
    /// <example>Failed to delete the specified S3 bucket</example>
    public string? ErrorMessageOnFailure { get; set; }
}
