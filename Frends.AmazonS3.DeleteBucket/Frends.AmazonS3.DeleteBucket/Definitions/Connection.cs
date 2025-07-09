using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// AWS Access Key ID.
    /// </summary>
    /// <example>AKIAQWERTY7NJ5Q7NZ6Q</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string AwsAccessKeyId { get; set; }

    /// <summary>
    /// AWS Secret Access Key.
    /// </summary>
    /// <example>TVh5hgd3uGY/2CqH+Kkrrg3dadbXLsYe0jC3h+WD</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string AwsSecretAccessKey { get; set; }

    /// <summary>
    /// AWS S3 bucket's region.
    /// </summary>
    /// <example>EuCentral1</example>
    public Region Region { get; set; }
}
