using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.ListObjectVersions.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Limits the response to keys that begin with the specified prefix.
    /// </summary>
    /// <example>bucket/object1/object2</example>
    public string Prefix { get; set; }

    /// <summary>
    /// Sets the maximum number of keys returned in one response.
    /// The response might contain fewer keys but will never contain more than this value.
    /// Multiple responses will be merged into one result.
    /// </summary>
    /// <example>1000</example>
    [DefaultValue(1000)]
    public int MaxKeys { get; set; } = 1000;

    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Custom error message</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
