using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Frends.AmazonS3.UploadObject.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AmazonS3.UploadObject.Tests;

[TestClass]
public class PreSignedUnitTests
{
    private readonly string? accessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_AccessKey");
    private readonly string? secretAccessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_SecretAccessKey");
    private readonly string? bucketName = Environment.GetEnvironmentVariable("HiQ_AwsS3Test_BucketName");
    private readonly string dir = Path.Combine(Environment.CurrentDirectory);

    private Connection? connection;
    private Input? input;
    private Options? options;

    [TestInitialize]
    public void Initialize()
    {
        Directory.CreateDirectory(Path.Combine(dir, "AWS"));
        File.AppendAllText(Path.Combine(dir, "AWS", "deletethis_presign.txt"), "Resource file deleted. (Presign)");
    }

    [TestCleanup]
    public void CleanUp()
    {
        if (Directory.Exists(Path.Combine(dir, "AWS")))
            Directory.Delete(Path.Combine(dir, "AWS"), true);

        using var sw = new StringWriter();
        using var client = new AmazonS3Client(accessKey, secretAccessKey, RegionEndpoint.EUCentral1);

        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = "Upload2023/PreSigned/UploadTest.txt"
        };
        client.DeleteObjectAsync(deleteObjectRequest);
    }

    [TestMethod]
    public async Task PreSignedUnitTest_UploadObject()
    {
        var setS3Key = Path.Combine("Upload2023", "PreSigned", "UploadTest.txt").Replace("\\", "/");

        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = null,
            BucketName = null,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.PreSignedUrl,
            PreSignedUrl = CreatePresignedUrl(setS3Key).ToString(),
            AwsAccessKeyId = null,
            AwsSecretAccessKey = null,
            Region = default,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = false,
            Acl = default,
            UseAcl = false,
        };
        options = new Options
        {
            ThrowErrorIfNoMatch = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(input, connection, options, CancellationToken.None);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_presign.txt")));
    }

    [TestMethod]
    public async Task PreSignedUnitTest_MissingURL_ThrowExceptionOnErrorResponse_false()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = null,
            BucketName = null,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.PreSignedUrl,
            PreSignedUrl = " ",
            AwsAccessKeyId = null,
            AwsSecretAccessKey = null,
            Region = default,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            Acl = default,
            UseAcl = false,
        };

        options = new Options
        {
            ThrowErrorIfNoMatch = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(input, connection, options, CancellationToken.None);
        Assert.AreEqual(0, result.Objects.Count);
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.DebugLog.Contains("Invalid URI: The format of the URI could not be determined"));
    }

    [TestMethod]
    public async Task PreSignedUnitTest_MissingURL_ThrowErrorOnFailure_true()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = null,
            BucketName = null,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.PreSignedUrl,
            PreSignedUrl = " ",
            AwsAccessKeyId = null,
            AwsSecretAccessKey = null,
            Region = default,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            Acl = default,
            UseAcl = false,
        };

        options = new Options
        {
            ThrowErrorIfNoMatch = false,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.UploadObject(input, connection, options, CancellationToken.None));
        Assert.IsTrue(ex.Message.Contains("Invalid URI: The format of the URI could not be determined"));
    }

    private Uri CreatePresignedUrl(string key)
    {
        var region = RegionEndpoint.EUCentral1;
        var client = new AmazonS3Client(accessKey, secretAccessKey, region);
        GetPreSignedUrlRequest request = new()
        {
            BucketName = bucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(15),

        };
        return new Uri(client.GetPreSignedURL(request));
    }
}
