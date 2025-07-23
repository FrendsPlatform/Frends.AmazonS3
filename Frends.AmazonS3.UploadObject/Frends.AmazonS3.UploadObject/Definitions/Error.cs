namespace Frends.AmazonS3.UploadObject.Definitions;

/// <summary>
/// Error information.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Upload failed</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional error information.
    /// </summary>
    /// <example>Connection timeout occurred</example>
    public dynamic AdditionalInfo { get; set; }
}
