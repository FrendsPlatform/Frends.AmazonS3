using System.ComponentModel;

namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Timeout in seconds.
    /// </summary>
    /// <example>5</example>
    public int Timeout { get; set; }

    /// <summary>
    /// Throw error on failure.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ThrowErrorOnFailure { get; set; }

    /// <summary>
    /// Error message on failure.
    /// </summary>
    /// <example></example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; }
}
