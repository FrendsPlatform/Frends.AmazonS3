using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DeleteBucket.Definitions;

/// <summary>
/// Connection parameters for AWS S3 authentication and region configuration.
/// </summary>
public class Connection
{
    /// <summary>
    /// AWS Access Key ID for authentication.
    /// This is the public part of your AWS credentials.
    /// </summary>
    /// <example>AKIAQWERTY7NJ5Q7NZ6Q</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string AwsAccessKeyId { get; set; }

    /// <summary>
    /// AWS Secret Access Key for authentication.
    /// This is the private part of your AWS credentials and should be kept secure.
    /// </summary>
    /// <example>TVh5hgd3uGY/2CqH+Kkrrg3dadbXLsYe0jC3h+WD</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string AwsSecretAccessKey { get; set; }

    /// <summary>
    /// AWS region where the S3 bucket is located.
    /// Must match the region where the bucket was created.
    /// </summary>
    /// <example>EuCentral1</example>
    public Region Region { get; set; }
}
