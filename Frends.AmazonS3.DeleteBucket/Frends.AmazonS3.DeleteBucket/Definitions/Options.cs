using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Options for configuring the behavior of the DeleteBucket task.
/// Controls error handling and custom error messaging.
/// </summary>
public class Options
{
    /// <summary>
    /// Determines whether to throw an exception when the operation fails.
    /// If set to false, errors will be returned in the Result object instead of throwing exceptions.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Custom error message to use when the operation fails.
    /// If not provided or empty, the original error message from the exception will be used.
    /// This allows for more user-friendly or localized error messages.
    /// </summary>
    /// <example>Failed to delete the specified S3 bucket</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue(null)]
    public string? ErrorMessageOnFailure { get; set; }
}
