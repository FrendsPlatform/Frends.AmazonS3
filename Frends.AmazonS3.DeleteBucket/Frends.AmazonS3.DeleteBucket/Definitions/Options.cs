using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Options for the DeleteBucket task.
/// </summary>
public class Options
{
    /// <summary>
    /// Determines whether to throw an error when the operation fails.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ThrowErrorOnFailure { get; set; } = false;

    /// <summary>
    /// Custom error message to use when the operation fails and ThrowErrorOnFailure is true.
    /// If empty, the default error message will be used.
    /// </summary>
    /// <example></example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = "";
}
