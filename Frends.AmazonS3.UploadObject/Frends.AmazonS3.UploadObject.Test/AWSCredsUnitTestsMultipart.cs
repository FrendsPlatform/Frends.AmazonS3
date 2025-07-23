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
public class AWSCredsUnitTestsMultipart
{
    public TestContext? TestContext { get; set; }
    private readonly string? _accessKey;
    private readonly string? _secretAccessKey;
    private readonly string? _bucketName;
    private readonly string _dir = Path.Combine(Environment.CurrentDirectory);
    private Connection _connection = new();
    private Input _input = new();
    private Options _options = new();

    public AWSCredsUnitTestsMultipart()
    {
        DotEnv.Load();
        _accessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_AccessKey");
        _secretAccessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_SecretAccessKey");
        _bucketName = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_BucketName");
    }

    [TestInitialize]
    public void Initialize()
    {
        _input = new Input
        {
            SourceDirectory = Path.Combine(_dir, "AWS"),
            ACL = default,
            FileMask = null,
            UseACL = false,
            TargetDirectory = "Upload2023/",
            PartSize = 100,
            BucketName = _bucketName,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };

        _connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.AWSCredentials,
            PreSignedURL = null,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = true,
            GatherDebugLog = true
        };

        _options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        Directory.CreateDirectory(Path.Combine(_dir, "AWS"));

        var fileList = new List<string>
        {
            Path.Combine(_dir, "AWS", "test1.txt"),
            Path.Combine(_dir, "AWS", "test2")
        };
        long targetSizeInBytes = 6L * 1024L * 1024L; // 6 GB in bytes

        foreach (var file in fileList)
            if (!File.Exists(file))
                CreateDummyFile(file, targetSizeInBytes);
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        if (Directory.Exists($@"{_dir}\AWS"))
            Directory.Delete($@"{_dir}\AWS", true);

        using var client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);
        var listObjectRequest = new ListObjectsRequest { BucketName = _bucketName };
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
        var result = await AmazonS3.UploadObject(_input, _connection, _options, default);
        Assert.AreEqual(2, result.Objects.Count);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.DebugLog);
        Assert.IsTrue(result.Objects.Any(x => x.Contains("test1.txt")));
    }

    [TestMethod]
    public async Task AWSCreds_Missing_ThrowExceptionOnErrorResponse_False()
    {
        var connection = _connection;
        connection.AwsAccessKeyId = null;
        connection.AwsSecretAccessKey = "";

        var options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.UploadObject(_input, connection, options, default);
        Assert.AreEqual(0, result.Objects.Count);
        Assert.IsFalse(result.Success, result.DebugLog);
        Assert.IsTrue(result.DebugLog.Contains("Please authenticate"));
    }

    [TestMethod]
    public async Task AWSCreds_Missing_ThrowExceptionOnErrorResponse_True()
    {
        var connection = _connection;
        connection.AwsAccessKeyId = null;
        connection.AwsSecretAccessKey = "";
        var options = new Options
        {
            ThrowErrorIfNoMatch = false,
            FailOnErrorResponse = true,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<UploadException>(async () => await AmazonS3.UploadObject(_input, connection, options, default));
        Assert.IsTrue(ex.DebugLog.Contains("Please authenticate"));
    }

    static void CreateDummyFile(string filePath, long targetSizeInBytes)
    {
        const int bufferSize = 1024 * 1024; // 1 MB buffer size
        byte[] buffer = new byte[bufferSize];
        var random = new Random();

        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
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
}
