using System.Collections.Generic;

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
    /// List of objects that encountered errors during delete operation.
    /// </summary>
    /// <example>Additional context or details</example>
    public List<SingleResultObject> AdditionalInfo { get; set; }
}
