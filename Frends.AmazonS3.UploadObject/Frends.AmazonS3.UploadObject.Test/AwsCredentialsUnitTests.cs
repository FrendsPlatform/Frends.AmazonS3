using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Frends.AmazonS3.UploadObject.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AmazonS3.UploadObject.Tests;

[TestClass]
public class AwsCredentialsUnitTests : AwsS3TestBase
{
    private readonly string dir = Path.Combine(Environment.CurrentDirectory);
    private Connection? connection;
    private Input? input;
    private Options? options;

    [TestInitialize]
    public void Initialize()
    {
        Directory.CreateDirectory(Path.Combine(dir, "AWS"));
        Directory.CreateDirectory(Path.Combine(dir, "AWS", "Subfolder"));
        Directory.CreateDirectory(Path.Combine(dir, "AWS", "EmptyFolder"));

        File.AppendAllText(Path.Combine(dir, "AWS", "test1.txt"), "test1");
        File.AppendAllText(Path.Combine(dir, "AWS", "Subfolder", "subfile.txt"), "From subfolder.");
        File.AppendAllText(Path.Combine(dir, "AWS", "deletethis_awscreds.txt"), "Resource file deleted. (AWS Creds)");
        File.AppendAllText(Path.Combine(dir, "AWS", "overwrite_presign.txt"), "Not overwriten. (Presign)");
        File.AppendAllText(Path.Combine(dir, "AWS", "overwrite_awscreds.txt"), "Not overwriten. (AWS creds)");
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        if (Directory.Exists(Path.Combine(dir, "AWS")))
            Directory.Delete(Path.Combine(dir, "AWS"), true);

        using var client = new AmazonS3Client(AccessKey, SecretAccessKey, RegionEndpoint.EUCentral1);

        var listObjectRequest = new ListObjectsRequest
        {
            BucketName = BucketName,
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

    [DataTestMethod]
    [DataRow("Upload2023")]
    [DataRow("Upload2023/")]
    public async Task AWSCreds_Upload(string targetDirectory)
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = targetDirectory,
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = AccessKey,
            AwsSecretAccessKey = SecretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
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
        Assert.AreEqual(5, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_Upload_GatherDebugLog_False()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = AccessKey,
            AwsSecretAccessKey = SecretAccessKey,
            Region = Region.EuCentral1,
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
        Assert.AreEqual(5, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_Missing_ThrowErrorOnFailure_False()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
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
        Assert.IsTrue(result.DebugLog.Contains("Access Denied"));
    }

    [TestMethod]
    public async Task AWSCreds_Missing_ThrowErrorOnFailure_True()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
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
            Acl = default,
            UseAcl = false,
        };

        options = new Options
        {
            ThrowErrorIfNoMatch = false,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = ""
        };

        await Assert.ThrowsExceptionAsync<Exception>(async () =>
            await AmazonS3.UploadObject(input, connection, options, CancellationToken.None));
    }

    [TestMethod]
    public async Task AWSCreds_UploadFromCurrentDirectoryOnly()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = true,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = AccessKey,
            AwsSecretAccessKey = SecretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
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
        Assert.AreEqual(4, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_Overwrite()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = AccessKey,
            AwsSecretAccessKey = SecretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = true,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
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
        Assert.AreEqual(5, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_PreserveFolderStructure()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = true,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = AccessKey,
            AwsSecretAccessKey = SecretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
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
        Assert.AreEqual(5, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_ReturnListOfObjectKeys()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = null,
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = AccessKey,
            AwsSecretAccessKey = SecretAccessKey,
            Region = Region.EuCentral1,
            ReturnListOfObjectKeys = true,
            Overwrite = true,
            UseMultipartUpload = false,
            GatherDebugLog = true,
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
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = fileName,
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = true,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = AccessKey,
            AwsSecretAccessKey = SecretAccessKey,
            Region = Region.EuCentral1,
            ReturnListOfObjectKeys = false,
            Overwrite = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
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
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("deletethis_awscreds.txt")));
        Assert.IsFalse(File.Exists(Path.Combine(dir, "AWS", fileName)));
    }

    [TestMethod]
    public async Task AWSCreds_ThrowErrorIfNoMatch()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS"),
            FileMask = "notafile*",
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = AccessKey,
            AwsSecretAccessKey = SecretAccessKey,
            Region = Region.EuCentral1,
            ReturnListOfObjectKeys = false,
            Overwrite = false,
            UseMultipartUpload = false,
            Acl = default,
            UseAcl = false,
        };

        options = new Options
        {
            ThrowErrorIfNoMatch = true,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () =>
            await AmazonS3.UploadObject(input, connection, options, CancellationToken.None));
        Assert.IsTrue(ex.Message.Contains($"No files match the filemask '{input.FileMask}' within supplied path."));
    }

    [TestMethod]
    public async Task AWSCreds_ACLs()
    {
        // Public ACLs like PublicRead, PublicReadWrite and AuthenticatedRead are
        // excluded from automated unit tests due to security
        var acls = new List<ACLs>
            { ACLs.Private, ACLs.BucketOwnerRead, ACLs.BucketOwnerFullControl, ACLs.LogDeliveryWrite };

        options = new Options
        {
            ThrowErrorIfNoMatch = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        foreach (var acl in acls)
        {
            input = new Input
            {
                SourceDirectory = Path.Combine(dir, "AWS"),
                FileMask = null,
                TargetDirectory = "Upload2023/",
                BucketName = BucketName,
                UploadFromCurrentDirectoryOnly = false,
                PreserveFolderStructure = false,
                DeleteSource = false,
            };

            connection = new Connection
            {
                AuthenticationMethod = AuthenticationMethod.AwsCredentials,
                PreSignedUrl = null,
                AwsAccessKeyId = AccessKey,
                AwsSecretAccessKey = SecretAccessKey,
                Region = Region.EuCentral1,
                Overwrite = false,
                ReturnListOfObjectKeys = false,
                UseMultipartUpload = false,
                GatherDebugLog = true,
                Acl = acl,
                UseAcl = true,
            };

            var result = await AmazonS3.UploadObject(input, connection, options, CancellationToken.None);
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
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS", "EmptyFolder"),
            FileMask = null,
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = AccessKey,
            AwsSecretAccessKey = SecretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
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
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
    }

    [TestMethod]
    public async Task AWSCreds_Upload_ShouldThrow_IfEmptyFolder_AndThrowErrorIfNoMatchIsTrue()
    {
        input = new Input
        {
            SourceDirectory = Path.Combine(dir, "AWS", "EmptyFolder"),
            FileMask = null,
            TargetDirectory = "Upload2023/",
            BucketName = BucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AwsCredentials,
            PreSignedUrl = null,
            AwsAccessKeyId = AccessKey,
            AwsSecretAccessKey = SecretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            Acl = default,
            UseAcl = false,
        };

        options = new Options
        {
            ThrowErrorIfNoMatch = true,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () =>
            await AmazonS3.UploadObject(input, connection, options, CancellationToken.None));
        Assert.IsTrue(ex.Message.Contains($"No files match the filemask '*' within supplied path."));
    }
}
