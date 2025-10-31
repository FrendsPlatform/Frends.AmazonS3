namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Represents a single S3 object to be deleted, containing bucket name, key, and optional version ID.
/// </summary>
public class S3ObjectArray
{
    /// <summary>
    /// The name of the AWS S3 bucket containing the object to delete.
    /// </summary>
    /// <example>my-example-bucket</example>
    public string BucketName { get; set; }

    /// <summary>
    /// The unique key (path/filename) identifying the object to delete within the S3 bucket.
    /// </summary>
    /// <example>folder/myfile.txt</example>
    public string Key { get; set; }

    /// <summary>
    /// Version ID used to reference a specific version of the object in a versioned bucket.
    /// Leave empty or null to delete the current version of the object.
    /// </summary>
    /// <example>q97fnr1zy_gsDcPAMbbwoW2eY0wgoFPt</example>
    public string VersionId { get; set; }
}
