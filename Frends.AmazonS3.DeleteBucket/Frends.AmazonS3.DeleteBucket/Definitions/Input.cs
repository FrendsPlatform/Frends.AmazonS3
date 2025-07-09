using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Input parameters for DeleteBucket task.
/// </summary>
public class Input
{
    /// <summary>
    /// Name of the S3 bucket to delete.
    /// </summary>
    /// <example>my-bucket-name</example>
    [DisplayName("Bucket Name")]
    [DefaultValue("")]
    [Required]
    public string BucketName { get; set; } = "";
}
