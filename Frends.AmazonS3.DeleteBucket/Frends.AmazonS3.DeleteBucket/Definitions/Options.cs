using System.ComponentModel;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Options for configuring error handling behavior in the DeleteBucket task.
/// </summary>
public class Options
{
    /// <summary>
    /// Determines whether to throw an exception when the operation fails.
    /// If false, errors will be returned in the Result object instead of throwing exceptions.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; }

    /// <summary>
    /// Custom error message to use when an error occurs.
    /// If empty, a default error message will be used.
    /// This message will be included in both thrown exceptions and Error objects.
    /// </summary>
    /// <example>Failed to delete the specified S3 bucket</example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = "";
}
