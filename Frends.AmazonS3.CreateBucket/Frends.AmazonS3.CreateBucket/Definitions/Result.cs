using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.CreateBucket.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// Region the bucket resides in.
    /// Return "Bucket already exists" if bucket already exists.
    /// </summary>
    /// <example>eu-central-1</example>
    public string BucketLocation { get; set; }

    /// <summary>
    /// AWS S3 bucket's name.
    /// See https://docs.aws.amazon.com/awscloudtrail/latest/userguide/cloudtrail-s3-bucket-naming-requirements.html
    /// </summary>
    /// <example>bucket</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BucketName { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; set; }
}
