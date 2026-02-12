namespace Frends.AmazonS3.ListObjectVersions.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// The name of the S3 bucket to list object versions from.
    /// </summary>
    /// <example>my-s3-bucket</example>
    public string BucketName { get; set; }
}
