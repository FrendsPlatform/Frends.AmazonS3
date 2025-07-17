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
}
