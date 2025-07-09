using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Options for the DeleteBucket task.
/// </summary>
public class Options
{
    /// <summary>
    /// Determines whether to throw an error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Custom error message to use when ThrowErrorOnFailure is false and an error occurs.
    /// If not provided, the original error message will be used.
    /// </summary>
    /// <example>Custom error message</example>
    public string? ErrorMessageOnFailure { get; set; }
}
