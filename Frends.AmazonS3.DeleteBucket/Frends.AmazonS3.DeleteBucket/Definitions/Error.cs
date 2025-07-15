using System.ComponentModel;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Error information for Amazon S3 operations.
/// </summary>
public class Error
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <value>The error message describing what went wrong.</value>
    [DisplayName("Message")]
    [Description("The error message describing what went wrong.")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional information about the error.
    /// </summary>
    /// <value>Additional details or context about the error.</value>
    [DisplayName("Additional Info")]
    [Description("Additional details or context about the error.")]
    public string AdditionalInfo { get; set; } = string.Empty;
}
