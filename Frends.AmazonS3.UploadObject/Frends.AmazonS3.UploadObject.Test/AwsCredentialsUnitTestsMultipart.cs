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
public class AwsCredentialsUnitTestsMultipart : AwsS3TestBase
{
    private readonly string dir = Path.Combine(Environment.CurrentDirectory);
    private Connection connection = new();
    private Input input = new();
    private Options options = new();

    [TestInitialize]
    public void Initialize()
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
            UseMultipartUpload = true,
            GatherDebugLog = true,
            Acl = default,
            UseAcl = false,
            PartSize = 100,
        };

        options = new Options
        {
            ThrowErrorIfNoMatch = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        Directory.CreateDirectory(Path.Combine(dir, "AWS"));

        var fileList = new List<string>
        {
            Path.Combine(dir, "AWS", "test1.txt"),
            Path.Combine(dir, "AWS", "test2")
        };
        long targetSizeInBytes = 6L * 1024L * 1024L; // 6 GB in bytes

        foreach (var file in fileList)
            if (!File.Exists(file))
                CreateDummyFile(file, targetSizeInBytes);
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        var awsDirectory = Path.Combine(dir, "AWS");
        if (Directory.Exists(awsDirectory))
            Directory.Delete(awsDirectory, true);

        using var client = new AmazonS3Client(AccessKey, SecretAccessKey, RegionEndpoint.EUCentral1);
        var listObjectRequest = new ListObjectsRequest
        {
            BucketName = BucketName
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
    public async Task AwsCredentials_Upload()
    {
        var result = await AmazonS3.UploadObject(input, connection, options, CancellationToken.None);
        Assert.AreEqual(2, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("test1.txt")));
    }

    [TestMethod]
    public async Task AwsCredentials_Missing_FailOnErrorResponse_False()
    {
        connection.AwsAccessKeyId = null;
        connection.AwsSecretAccessKey = "";

        options = new Options
        {
            ThrowErrorIfNoMatch = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(input, connection, options, CancellationToken.None);
        Assert.AreEqual(0, result.Objects.Count);
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.DebugLog.Contains("Please authenticate"));
    }

    [TestMethod]
    public async Task AwsCredentials_Missing_ThrowErrorOnFailure_True()
    {
        connection.AwsAccessKeyId = null;
        connection.AwsSecretAccessKey = "";
        options = new Options
        {
            ThrowErrorIfNoMatch = false,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () =>
            await AmazonS3.UploadObject(input, connection, options, CancellationToken.None));
        Assert.IsTrue(ex.Message.Contains("Please authenticate"));
    }

    private static void CreateDummyFile(string filePath, long targetSizeInBytes)
    {
        const int bufferSize = 1024 * 1024; // 1 MB buffer size
        byte[] buffer = new byte[bufferSize];
        var random = new Random();

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        long bytesWritten = 0;

        while (bytesWritten < targetSizeInBytes)
        {
            random.NextBytes(buffer);
            int bytesToWrite = (int)Math.Min(buffer.Length, targetSizeInBytes - bytesWritten);
            fileStream.Write(buffer, 0, bytesToWrite);
            bytesWritten += bytesToWrite;

            Console.WriteLine($"Progress: {bytesWritten / (1024 * 1024)} MB / 6144 MB");
        }
    }
}
