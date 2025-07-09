using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Connection parameters for AWS S3 authentication and region configuration.
/// </summary>
public class Connection
{
    /// <summary>
    /// AWS Access Key ID used for authentication with AWS S3 services.
    /// This is a unique identifier that AWS uses to identify your account.
    /// </summary>
    /// <example>AKIAQWERTY7NJ5Q7NZ6Q</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string AwsAccessKeyId { get; set; }

    /// <summary>
    /// AWS Secret Access Key used for authentication with AWS S3 services.
    /// This is a secret key that should be kept confidential and is used together with the Access Key ID.
    /// </summary>
    /// <example>TVh5hgd3uGY/2CqH+Kkrrg3dadbXLsYe0jC3h+WD</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string AwsSecretAccessKey { get; set; }

    /// <summary>
    /// AWS S3 bucket's region where the bucket is located.
    /// This determines which AWS region endpoint will be used for the S3 operations.
    /// </summary>
    /// <example>EuCentral1</example>
    [DefaultValue(Region.EuWest1)]
    public Region Region { get; set; } = Region.EuWest1;
}
