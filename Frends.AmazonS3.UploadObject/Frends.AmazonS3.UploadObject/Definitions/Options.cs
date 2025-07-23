using System.ComponentModel;

namespace Frends.AmazonS3.UploadObject.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Throw error if there are no object(s) in the path matching the filemask.
    /// </summary>
    /// <example>false</example>
    public bool ThrowErrorIfNoMatch { get; set; }

    /// <summary>
    /// Throws exception if error occures in upload.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool FailOnErrorResponse { get; set; }

    /// <summary>
    /// Throw error on failure.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ThrowErrorOnFailure { get; set; }

    /// <summary>
    /// Error message to display on failure.
    /// </summary>
    /// <example></example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = "";
}
