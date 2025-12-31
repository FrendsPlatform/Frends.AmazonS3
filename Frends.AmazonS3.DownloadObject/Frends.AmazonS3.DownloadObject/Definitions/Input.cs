using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DownloadObject.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// AWS S3 bucket's name.
    /// </summary>
    /// <example>Bucket</example>
    public string BucketName { get; set; }

    /// <summary>
    /// Downloads all objects with this prefix.
    /// </summary>
    /// <example>directory/</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string SourceDirectory { get; set; }

    /// <summary>
    /// String pattern to search objects.
    /// </summary>
    /// <example>*.*, *file?.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("*")]
    public string SearchPattern { get; set; }

    /// <summary>
    /// Set to true to download objects from the current directory only.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(true)]
    public bool DownloadFromCurrentDirectoryOnly { get; set; }

    /// <summary>
    /// Destination directory where to create folders and files.
    /// </summary>
    /// <example>c:\temp, \\network\folder</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string TargetDirectory { get; set; }
}