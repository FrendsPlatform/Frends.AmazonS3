using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// AWS S3 bucket's name.
    /// See https://docs.aws.amazon.com/awscloudtrail/latest/userguide/cloudtrail-s3-bucket-naming-requirements.html
    /// </summary>
    /// <example>bucket</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BucketName { get; set; }
}
