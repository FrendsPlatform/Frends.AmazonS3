namespace Frends.AmazonS3.ListObjectVersions.Definitions;

/// <summary>
/// Object data.
/// </summary>
public class BucketObjectVersions
{
    /// <summary>
    /// The name of the bucket containing this object.
    /// </summary>
    /// <example>ObjectName</example>
    public string BucketName { get; set; }

    /// <summary>
    /// The key of the object.
    /// </summary>
    /// <example>ObjectDir/ObjectName</example>
    public string Key { get; set; }

    /// <summary>
    /// Entity tag (ETag) - a hash of the object used for change detection.
    /// </summary>
    /// <example>2b9338cfb801ca193ee45d49acc2ba99</example>
    public string ETag { get; set; }

    /// <summary>
    /// Version ID of an object.
    /// </summary>
    /// <example>1234</example>
    public string Version { get; set; }
}
