using System;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.CreateBucket.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to join strings.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>object { Exception Exception }</example>
    public object AdditionalInfo { get; set; }
}

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Region the bucket resides in.
    /// Return "Bucket already exists" if bucket already exists.
    /// </summary>
    /// <example>eu-central-1</example>
    public string BucketLocation { get; private set; }

    /// <summary>
    /// AWS S3 bucket's name.
    /// See https://docs.aws.amazon.com/awscloudtrail/latest/userguide/cloudtrail-s3-bucket-naming-requirements.html
    /// </summary>
    /// <example>bucket</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BucketName { get; private set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; private set; }

    internal Result(bool success, string bucketLocation, string bucketName, Error error = null)
    {
        Success = success;
        BucketLocation = bucketLocation;
        BucketName = bucketName;
        Error = error;

    }
}
