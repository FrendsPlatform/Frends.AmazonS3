using System.ComponentModel;

namespace Frends.AmazonS3.ListObjectVersions.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// AWS Access Key ID used for authentication with AWS services.
    /// </summary>
    /// <example>AKIAQWERTY7NJ5Q7NZ6Q</example>
    [PasswordPropertyText]
    public string AwsAccessKeyId { get; set; }

    /// <summary>
    /// AWS Secret Access Key used for authentication with AWS services.
    /// </summary>
    /// <example>TVh5hgd3uGY/2CqH+Kkrrg3dadbXLsYe0jC3h+WD</example>
    [PasswordPropertyText]
    public string AwsSecretAccessKey { get; set; }

    /// <summary>
    /// AWS Region where the S3 bucket is located.
    /// </summary>
    /// <example>EuCentral1</example>
    public Region Region { get; set; }
}
