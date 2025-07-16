using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Input parameters for the DeleteBucket task.
/// </summary>
public class Input
{
    /// <summary>
    /// AWS S3 bucket's name to be deleted.
    /// Must follow AWS S3 bucket naming requirements.
    /// See https://docs.aws.amazon.com/awscloudtrail/latest/userguide/cloudtrail-s3-bucket-naming-requirements.html
    /// </summary>
    /// <example>my-example-bucket</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BucketName { get; set; }
}
