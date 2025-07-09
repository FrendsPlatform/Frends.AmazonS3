using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Connection parameters for AWS S3 operations.
/// </summary>
public class Connection
{
    /// <summary>
    /// AWS Access Key ID for authentication.
    /// </summary>
    /// <example>AKIAQWERTY7NJ5Q7NZ6Q</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    [Required]
    [DefaultValue("")]
    public string AwsAccessKeyId { get; set; } = "";

    /// <summary>
    /// AWS Secret Access Key for authentication.
    /// </summary>
    /// <example>TVh5hgd3uGY/2CqH+Kkrrg3dadbXLsYe0jC3h+WD</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    [Required]
    [DefaultValue("")]
    public string AwsSecretAccessKey { get; set; } = "";

    /// <summary>
    /// AWS region where the S3 operations will be performed.
    /// </summary>
    /// <example>EuCentral1</example>
    [DefaultValue(Region.EuWest1)]
    public Region Region { get; set; } = Region.EuWest1;
}
