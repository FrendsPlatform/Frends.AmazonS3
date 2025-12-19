using System.ComponentModel;

namespace Frends.AmazonS3.DownloadObject.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Delete the S3 source object after download.
    /// Subfolders will also be deleted if they are part of the object's key and there are no objects left.
    /// Create subfolders manually to make sure they won't be deleted.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool DeleteSourceObject { get; set; }

    /// <summary>
    /// Throw an error if there are no objects in the path matching the search pattern.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(true)]
    public bool ThrowErrorIfNoMatch { get; set; }

    /// <summary>
    /// Actions if a destination file already exists.
    /// </summary>
    /// <example>Info</example>
    [DefaultValue(DestinationFileExistsActions.Info)]
    public DestinationFileExistsActions ActionOnExistingFile { get; set; }

    /// <summary>
    /// For how long will this Task try to write to a locked file.
    /// Value in seconds.
    /// </summary>
    /// <example>10</example>
    [DefaultValue(10)]
    public int FileLockedRetries { get; set; }

    /// <summary>
    /// Throw an error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Error message to display on failure.
    /// </summary>
    /// <example>Can't download an object</example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; }
}