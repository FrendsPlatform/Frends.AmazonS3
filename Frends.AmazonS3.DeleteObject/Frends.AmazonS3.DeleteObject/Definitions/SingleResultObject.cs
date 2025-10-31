namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Represents the result of a single delete operation, containing information about the processed object.
/// </summary>
public class SingleResultObject
{
    /// <summary>
    /// The name of the S3 bucket that contained the processed object.
    /// </summary>
    /// <example>my-example-bucket</example>
    public string BucketName { get; set; }

    /// <summary>
    /// The key (path/filename) that identified the processed object within the S3 bucket.
    /// </summary>
    /// <example>folder/myfile.txt</example>
    public string Key { get; set; }

    /// <summary>
    /// The version ID of the delete marker created as a result of the DELETE operation, or the version ID of the deleted object.
    /// Null if the object was not found (when ActionOnObjectNotFound is set to Info) or if the bucket is not versioned.
    /// </summary>
    /// <example>q97fnr1zy_gsDcPAMbbwoW2eY0wgoFPt</example>
    public string VersionId { get; set; }
}
