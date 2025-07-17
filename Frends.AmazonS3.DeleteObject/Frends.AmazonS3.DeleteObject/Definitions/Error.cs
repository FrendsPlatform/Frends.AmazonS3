namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Error information.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Object ExampleKey doesn't exist in ExampleBucket.</example>
    public string Message { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Object ExampleKey doesn't exist in ExampleBucket.</example>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Additional error information.
    /// </summary>
    /// <example>Additional context or details</example>
    public dynamic AdditionalInfo { get; set; }
}
