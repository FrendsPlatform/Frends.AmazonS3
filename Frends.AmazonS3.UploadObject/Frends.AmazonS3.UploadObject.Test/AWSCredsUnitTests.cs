using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Frends.AmazonS3.UploadObject.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;

namespace Frends.AmazonS3.UploadObject.Tests;

[TestClass]
public class AWSCredsUnitTests
{
    public TestContext? TestContext { get; set; }
    private readonly string? _accessKey;
    private readonly string? _secretAccessKey;
    private readonly string? _bucketName;
    private readonly string _dir = Path.Combine(Environment.CurrentDirectory);
    Connection? _connection;
    Input? _input;
    Options? _options;

    public AWSCredsUnitTests()
    {
        DotEnv.Load();
        _accessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_AccessKey");
        _secretAccessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_SecretAccessKey");
        _bucketName = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_BucketName");
    }

    [TestInitialize]
    public void Initialize()
    {
        Directory.CreateDirectory(Path.Combine(_dir, "AWS"));
        Directory.CreateDirectory(Path.Combine(_dir, "AWS", "Subfolder"));
        Directory.CreateDirectory(Path.Combine(_dir, "AWS", "EmptyFolder"));

        File.AppendAllText(Path.Combine(_dir, "AWS", "test1.txt"), "test1");
        File.AppendAllText(Path.Combine(_dir, "AWS", "Subfolder", "subfile.txt"), "From subfolder.");
        File.AppendAllText(Path.Combine(_dir, "AWS", "deletethis_awscreds.txt"), "Resource file deleted. (AWS Creds)");
        File.AppendAllText(Path.Combine(_dir, "AWS", "overwrite_presign.txt"), "Not overwriten. (Presign)");
        File.AppendAllText(Path.Combine(_dir, "AWS", "overwrite_awscreds.txt"), "Not overwriten. (AWS creds)");
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        if (Directory.Exists(Path.Combine(_dir, "AWS")))
            Directory.Delete(Path.Combine(_dir, "AWS"), true);

        using var client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);

        var listObjectRequest = new ListObjectsRequest
        {
            BucketName = _bucketName,
        };

        var response = await client.ListObjectsAsync(listObjectRequest);
        var objects = response.S3Objects;

        if (objects == null) return;
        foreach (var obj in objects)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = obj.BucketName,
                Key = obj.Key,
            };
            await client.DeleteObjectAsync(deleteObjectRequest);
        }
    }

    [TestMethod]
    public async Task AWSCreds_Upload()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
        Assert.AreEqual(5, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_Upload_GatherDebugLog_False()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = false,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
        Assert.AreEqual(5, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_Missing_ThrowExceptionOnErrorResponse_False()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = null,
            AwsSecretAccessKey = "",
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
        Assert.AreEqual(0, result.Objects.Count);
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.DebugLog.Contains("Access Denied"));
    }

    [TestMethod]
    public async Task AWSCreds_Missing_ThrowExceptionOnErrorResponse_True()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = null,
            AwsSecretAccessKey = "",
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = true,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<UploadException>(async () => await AmazonS3.UploadObject(_input, _connection, _options, default));
        Assert.IsTrue(ex.DebugLog.Contains("Access Denied"));
    }

    [TestMethod]
    public async Task AWSCreds_UploadFromCurrentDirectoryOnly()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = true,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
        Assert.AreEqual(4, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_Overwrite()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = true,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
        Assert.AreEqual(5, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_PreserveFolderStructure()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = true,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
        Assert.AreEqual(5, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_ReturnListOfObjectKeys()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            ReturnListOfObjectKeys = true,
            Overwrite = true,
            UseMultipartUpload = false,
            GatherDebugLog = true,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
        Assert.AreEqual(5, result.Objects.Count);
        Assert.IsFalse(result.Objects.Any(x => x.Contains("C:")));
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_DeleteSourceFile_Mask()
    {
        var fileName = "deletethis_awscreds.txt";
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = fileName,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = true,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            ReturnListOfObjectKeys = false,
            Overwrite = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
        Assert.IsFalse(File.Exists(Path.Combine(_dir, "AWS", fileName)));
    }

    [TestMethod]
    public async Task AWSCreds_ThrowErrorIfNoMatch()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = "notafile*",
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            ReturnListOfObjectKeys = false,
            Overwrite = false,
            UseMultipartUpload = false,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = true,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.UploadObject(_input, _connection, _options, default));
        Assert.IsTrue(ex.Message.Contains($"No files match the filemask '{_input.FileMask}' within supplied path."));
    }

    [TestMethod]
    public async Task AWSCreds_ACLs()
    {
        // Public ACLs like PublicRead, PublicReadWrite and AuthenticatedRead are
        // excluded from automated unit tests due to security
        var acls = new List<ACLs> { ACLs.Private, ACLs.BucketOwnerRead, ACLs.BucketOwnerFullControl, ACLs.LogDeliveryWrite };

        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true
        };

        foreach (var acl in acls)
        {
            _input = new Input
            {
                SourceDirectory = Path.Combine(_dir, "AWS"),
                ACL = acl,
                FileMask = null,
                UseACL = true,
                TargetDirectory = "Upload2023/",
                BucketName = _bucketName,
                UploadFromCurrentDirectoryOnly = false,
                PreserveFolderStructure = false,
                DeleteSource = false,
            };

            _options = new Options
            {
                ThrowErrorIfNoMatch = false,
                FailOnErrorResponse = false,
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = ""
            };

            var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
            Assert.AreEqual(5, result.Objects.Count, acl + Environment.NewLine + result.DebugLog);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.DebugLog);
            Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));

            await CleanUp();
            Initialize();
        }
    }

    [TestMethod]
    public async Task AWSCreds_Upload_ShouldNotThrow_IfEmptyFolder_AndThrowErrorIfNoMatchIsFalse()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS", "EmptyFolder"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
        Assert.AreEqual(0, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
    }

    [TestMethod]
    public async Task AWSCreds_Upload_ShouldThrow_IfEmptyFolder_AndThrowErrorIfNoMatchIsTrue()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS", "EmptyFolder"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = true,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.UploadObject(_input, _connection, _options, default));
        Assert.IsTrue(ex.Message.Contains($"No files match the filemask '*' within supplied path."));
    }

}
