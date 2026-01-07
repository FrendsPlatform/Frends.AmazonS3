using System.ComponentModel;

namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Connection parameters for establishing connection to AWS S3 service.
/// </summary>
public class Connection
{
    /// <summary>
    /// AWS Access Key ID for authentication with AWS S3 service.
    /// </summary>
    /// <example>AKIAQWERTY7NJ5Q7NZ6Q</example>
    [PasswordPropertyText]
    [DefaultValue("")]
    public string AwsAccessKeyId { get; set; }

    /// <summary>
    /// AWS Secret Access Key for authentication with AWS S3 service.
    /// </summary>
    /// <example>TVh5hgd3uGY/2CqH</example>
    [PasswordPropertyText]
    [DefaultValue("")]
    public string AwsSecretAccessKey { get; set; }

    /// <summary>
    /// The AWS region where the S3 bucket is located.
    /// </summary>
    /// <example>Region.EuCentral1</example>
    [DefaultValue(Region.EuWest1)]
    public Region Region { get; set; }
}
