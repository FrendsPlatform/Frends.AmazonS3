﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.UploadObject.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Source directory.
    /// </summary>
    /// <example>c:\temp, \\network\folder</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string SourceDirectory { get; set; }

    /// <summary>
    /// Windows-style filemask. Empty field = all objects (*).
    /// Only one object will be uploaded when using pre-signed URL. Consider using .zip (for example) when uploading multiple objects at the same time.
    /// </summary>
    /// <example>*.* , ?_file.*, foo_*.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("*")]
    public string FileMask { get; set; }

    /// <summary>
    /// AWS S3 root directory. If directory does not exist, it will be created.
    /// </summary>
    /// <example>directory/</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string TargetDirectory { get; set; }

    /// <summary>
    /// AWS S3 bucket's name.
    /// </summary>
    /// <example>Bucket</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BucketName { get; set; }

    /// <summary>
    /// Set to true to upload object(s) from current directory only.
    /// </summary>
    /// <example>false</example>
    public bool UploadFromCurrentDirectoryOnly { get; set; }

    /// <summary>
    /// Set to true to create subdirectories to S3 bucket.
    /// </summary>
    /// <example>false</example>
    public bool PreserveFolderStructure { get; set; }

    /// <summary>
    /// Delete local source object(s) after upload.
    /// </summary>
    /// <example>false</example>
    public bool DeleteSource { get; set; }

    /// <summary>
    /// Specifies the size (in MB) of individual parts into which large files are divided when Connection.UseMultipartUpload = true.
    /// Each part is limited to a minimum of 5 MB and a maximum of 5 TB in Amazon S3.
    /// Recommended part sizes typically range from 10 MB to 100 MB for optimal performance.
    /// </summary>
    /// <example>10</example>
    public long PartSize { get; set; }

    /// <summary>
    /// Enable/disable AWS S3 access control list.
    /// Not supported when using multipart upload (Connection.UseMultipartUpload = true).
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool UseACL { get; set; }

    /// <summary>
    /// Access control list. Enabled when UseACL is true.
    /// </summary>
    /// <example>Private</example>
    [UIHint(nameof(UseACL), "", true)]
    public ACLs ACL { get; set; }
}
