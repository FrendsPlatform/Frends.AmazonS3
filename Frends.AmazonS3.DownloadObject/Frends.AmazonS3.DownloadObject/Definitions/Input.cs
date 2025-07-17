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
    /// Set to true to download objects from current directory only.
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

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Authentication method to use when connecting to AWS S3 bucket. 
    /// </summary>
    /// <example>AwsCredentials</example>
    [DefaultValue(AuthenticationMethods.AwsCredentials)]
    public AuthenticationMethods AuthenticationMethod { get; set; }

    /// <summary>
    /// A pre-signed URL allows you to grant temporary access to users who don't have permission to directly run AWS operations in your account.
    /// </summary>
    /// <example>"https://bucket.s3.region.amazonaws.com/object/file.txt?X...</example>
    [PasswordPropertyText]
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethods.PreSignedUrl)]
    [DefaultValue(null)]
    public string PreSignedUrl { get; set; }

    /// <summary>
    /// AWS Access Key ID.
    /// </summary>
    /// <example>AKIAQWERTY7NJ5Q7NZ6Q</example>
    [PasswordPropertyText]
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethods.AwsCredentials)]
    [DefaultValue(null)]
    public string AwsAccessKeyId { get; set; }

    /// <summary>
    /// AWS Secret Access Key.
    /// </summary>
    /// <example>TVh5hgd3uGY/2CqH</example>
    [PasswordPropertyText]
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethods.AwsCredentials)]
    [DefaultValue(null)]
    public string AwsSecretAccessKey { get; set; }


    /// <summary>
    /// AWS S3 bucket's region.
    /// </summary>
    /// <example>EuCentral1</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethods.AwsCredentials)]
    public Region Region { get; set; }


}
