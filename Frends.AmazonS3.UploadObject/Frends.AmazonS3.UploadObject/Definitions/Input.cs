using System.ComponentModel;
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
    /// AWS S3 root directory. If the directory does not exist, it will be created.
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
    /// Set to true to upload object(s) from the current directory only.
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
}
