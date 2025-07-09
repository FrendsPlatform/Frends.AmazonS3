using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Options for configuring the DeleteBucket task behavior.
/// </summary>
public class Options
{
    /// <summary>
    /// Determines whether to throw an exception when the operation fails.
    /// When set to false, errors will be returned in the Result object instead of throwing exceptions.
    /// </summary>
    /// <example>true</example>
    [DisplayName("Throw Error On Failure")]
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Custom error message to use when ThrowErrorOnFailure is true and an error occurs.
    /// If not provided, the original error message will be used.
    /// </summary>
    /// <example>Custom error message for bucket deletion failure</example>
    [DisplayName("Custom Error Message")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = "";
}
