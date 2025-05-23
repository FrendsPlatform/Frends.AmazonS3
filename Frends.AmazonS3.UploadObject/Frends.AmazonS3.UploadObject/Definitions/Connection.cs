﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.UploadObject.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Authentication method to use when connecting to AWS S3 bucket. Options are pre-signed URL or AWS Access Key ID+AWS Secret Access Key.
    /// </summary>
    /// <example>PreSignedURL</example>
    [DefaultValue(AuthenticationMethod.AWSCredentials)]
    public AuthenticationMethod AuthenticationMethod { get; set; }

    /// <summary>
    /// A pre-signed URL allows you to grant temporary access to users who don't have permission to directly run AWS operations in your account.
    /// Enabled when using PreSignedURL.
    /// </summary>
    /// <example>"https://bucket.s3.region.amazonaws.com/object/file.txt?X-Amz-Expires=120X-Amz-Algorithm...</example>
    [PasswordPropertyText]
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.PreSignedURL)]
    public string PreSignedURL { get; set; }

    #region AWSCredentials

    /// <summary>
    /// Use Multipart upload?
    /// This is a method used to asynchronously initiate a multipart upload for a large object (file size over 5 GB) to Amazon S3.
    /// Multipart upload breaks the object into smaller parts, which are uploaded independently in parallel.
    /// </summary>
    /// <example>true</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.AWSCredentials)]
    [DefaultValue(true)]
    public bool UseMultipartUpload { get; set; }

    /// <summary>
    /// AWS Access Key ID. Enabled when using AWSCredentials.
    /// </summary>
    /// <example>AKIAQWERTY7NJ5Q7NZ6Q</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.AWSCredentials)]
    public string AwsAccessKeyId { get; set; }

    /// <summary>
    /// AWS Secret Access Key. Enabled when using AWSCredentials.
    /// </summary>
    /// <example>TVh5hgd3uGY/2CqH+Kkrrg3dadbXLsYe0jC3h+WD</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.AWSCredentials)]
    public string AwsSecretAccessKey { get; set; }

    /// <summary>
    /// AWS S3 bucket's name. Enabled when using AWSCredentials.
    /// </summary>
    /// <example>Bucket</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.AWSCredentials)]
    [DisplayFormat(DataFormatString = "Text")]
    public string BucketName { get; set; }

    /// <summary>
    /// AWS S3 bucket's region. Enabled when using AWSCredentials.
    /// </summary>
    /// <example>EuCentral1</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.AWSCredentials)]
    public Region Region { get; set; }
    #endregion AWSCredentials

    #region options
    /// <summary>
    /// Set to true to upload object(s) from current directory only.
    /// </summary>
    /// <example>false</example>
    public bool UploadFromCurrentDirectoryOnly { get; set; }

    /// <summary>
    /// Set to true to create subdirectories to S3 bucket. Enabled when UploadFromCurrentDirectoryOnly = false and using AWSCredentials.
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.AWSCredentials)]
    public bool PreserveFolderStructure { get; set; }

    /// <summary>
    /// Return object keys from S3 in prefix/filename-format. Enabled when using AWSCredentials.
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.AWSCredentials)]
    public bool ReturnListOfObjectKeys { get; set; }

    /// <summary>
    /// Set to true to overwrite object(s) with the same path and name (object key). Enabled when using AWSCredentials.
    /// </summary>
    /// <example>false</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.AWSCredentials)]
    public bool Overwrite { get; set; }

    /// <summary>
    /// Delete local source object(s) after upload.
    /// </summary>
    /// <example>false</example>
    public bool DeleteSource { get; set; }

    /// <summary>
    /// Whether to gather AWS SDK debug log.
    /// Please note that AWS SDK logging is not thread-safe and can pollute log output for tasks that run in parallel.
    /// It is recommended to disable debug logging in production.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool GatherDebugLog { get; set; }

    /// <summary>
    /// Throw error if there are no object(s) in the path matching the filemask.
    /// </summary>
    /// <example>false</example>
    public bool ThrowErrorIfNoMatch { get; set; }

    /// <summary>
    /// Throws exception if error occures in upload.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ThrowExceptionOnErrorResponse { get; set; }
    #endregion options
}