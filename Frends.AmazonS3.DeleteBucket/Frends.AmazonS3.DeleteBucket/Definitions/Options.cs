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
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ThrowErrorOnFailure { get; set; } = false;

    /// <summary>
    /// Custom error message to use when ThrowErrorOnFailure is true and an error occurs.
    /// </summary>
    /// <example></example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = "";
}
