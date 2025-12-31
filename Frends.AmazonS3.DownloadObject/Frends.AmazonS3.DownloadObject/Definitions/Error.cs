namespace Frends.AmazonS3.DownloadObject.Definitions;

/// <summary>
/// Error information.
/// </summary>
public class Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="additionalInfo">Additional error information</param>
    public Error(string message, dynamic additionalInfo = null)
    {
        Message = message;
        AdditionalInfo = additionalInfo;
    }

    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>An error occurred during processing</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional error information.
    /// </summary>
    /// <example>{ "ErrorCode": 500, "Details": "Connection timeout" }</example>
    public dynamic AdditionalInfo { get; set; }
}