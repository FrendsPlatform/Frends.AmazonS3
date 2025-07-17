using System.ComponentModel;

namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Connection parameters for AWS S3.
/// </summary>
public class Connection
{
    /// <summary>
    /// AWS Access Key ID.
    /// </summary>
    /// <example>AKIAQWERTY7NJ5Q7NZ6Q</example>
    [PasswordPropertyText]
    [DefaultValue("")]
    public string AwsAccessKeyId { get; set; }

    /// <summary>
    /// AWS Secret Access Key.
    /// </summary>
    /// <example>TVh5hgd3uGY/2CqH</example>
    [PasswordPropertyText]
    [DefaultValue("")]
    public string AwsSecretAccessKey { get; set; }

    /// <summary>
    /// The region to connect.
    /// </summary>
    /// <example>EuCentral1</example>
    [DefaultValue(Region.EuWest1)]
    public Region Region { get; set; }
}
