using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Amazon.S3;
using Amazon;
using Amazon.S3.Model;
using Frends.AmazonS3.DeleteObject.Definitions;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Frends.AmazonS3.DeleteObject.Tests;

[TestClass]
public class UnitTests
{
    private readonly string? _accessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_AccessKey");
    private readonly string? _secretAccessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_SecretAccessKey");
    private readonly string? _bucketName = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_BucketName");
    private readonly string _dir = Path.Combine(Environment.CurrentDirectory);

    [TestInitialize]
    public async Task Init()
    {
        var key = "ExampleFile";
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key, VersionId = null }, };

        if (!await FileExistsInS3(key))
            await CreateTestFiles(objects);
    }

    [TestCleanup]
    public void CleanUp()
    {
        if (Directory.Exists($@"{_dir}\TempFiles"))
            Directory.Delete($@"{_dir}\TempFiles", true);
    }

    [TestMethod]
    public async Task DeleteSingleObject_NoVersion()
    {
        var key = "Key1.txt";
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key, VersionId = null } };
        var handlers = new List<NotExistsHandler>() { NotExistsHandler.None, NotExistsHandler.Info, NotExistsHandler.Throw };

        foreach (var handler in handlers)
        {
            await CreateTestFiles(objects);

            var input = new Input()
            {
                Objects = objects,
                ActionOnObjectNotFound = handler,
            };

            var connection = new Connection()
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                Region = Region.EuCentral1,
            };

            var options = new Options()
            {
                Timeout = 1,
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = ""
            };

            var result = await AmazonS3.DeleteObject(input, connection, options, default);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(objects.Length, result.Data.Count);
            Assert.IsTrue(result.Data[0].Success);
            Assert.AreEqual(key, result.Data[0].Key);
            Assert.IsNotNull(result.Data[0].VersionId);
            Assert.IsNull(result.Data[0].Error);
            Assert.IsFalse(FileExistsInS3(key).Result);
            Assert.IsTrue(FileExistsInS3("ExampleFile").Result);

            CleanUp();
        }
    }

    [TestMethod]
    public async Task DeleteSingleObject_WrongRegions()
    {
        var key = "Key1.txt";
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key, VersionId = null } };
        var regions = new List<Region>() { Region.AfSouth1, Region.ApEast1, Region.ApNortheast1, Region.ApNortheast2, Region.ApNortheast3, Region.ApSouth1, Region.ApSoutheast1, Region.ApSoutheast2, Region.CaCentral1, Region.CnNorth1, Region.CnNorthWest1, Region.EuNorth1, Region.EuSouth1, Region.EuWest1, Region.EuWest2, Region.EuWest3, Region.MeSouth1, Region.SaEast1, Region.UsEast1, Region.UsEast2, Region.UsWest1, Region.UsWest2 };

        foreach (var region in regions)
        {
            await CreateTestFiles(objects);

            var input = new Input()
            {
                Objects = objects,
                ActionOnObjectNotFound = NotExistsHandler.None,
            };

            var connection = new Connection()
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                Region = region,
            };

            var options = new Options()
            {
                Timeout = 0,
                ThrowErrorOnFailure = true,
                ErrorMessageOnFailure = ""
            };

            var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.DeleteObject(input, connection, options, default));
            Assert.IsNotNull(ex);
        }

        CleanUp();
    }

    [TestMethod]
    public async Task DeleteSingleObject_Version()
    {
        var key = "Key1.txt";
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key, VersionId = "1" } };
        var handlers = new List<NotExistsHandler>() { NotExistsHandler.None, NotExistsHandler.Info, NotExistsHandler.Throw };

        foreach (var handler in handlers)
        {
            await CreateTestFiles(objects);

            var input = new Input()
            {
                Objects = objects,
                ActionOnObjectNotFound = handler,
            };

            var connection = new Connection()
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                Region = Region.EuCentral1,
            };

            var options = new Options()
            {
                Timeout = 1,
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = ""
            };

            var result = await AmazonS3.DeleteObject(input, connection, options, default);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(objects.Length, result.Data.Count);
            Assert.IsTrue(result.Data[0].Success);
            Assert.AreEqual(key, result.Data[0].Key);
            Assert.IsNotNull(result.Data[0].VersionId);
            Assert.IsNull(result.Data[0].Error);
            Assert.IsFalse(FileExistsInS3(key).Result);
            Assert.IsTrue(FileExistsInS3("ExampleFile").Result);

            CleanUp();
        }
    }

    [TestMethod]
    public async Task DeleteMultipleObjects()
    {
        var keys = new[] { "Key1.txt", "Key2.txt", "Key3.txt" };
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = keys[0] }, new S3ObjectArray { BucketName = _bucketName, Key = keys[1] }, new S3ObjectArray { BucketName = _bucketName, Key = keys[2] } };
        var handlers = new List<NotExistsHandler>() { NotExistsHandler.None, NotExistsHandler.Info, NotExistsHandler.Throw };

        foreach (var handler in handlers)
        {
            await CreateTestFiles(objects);

            var input = new Input()
            {
                Objects = objects,
                ActionOnObjectNotFound = handler,
            };

            var connection = new Connection()
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                Region = Region.EuCentral1,
            };

            var options = new Options()
            {
                Timeout = 1,
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = ""
            };

            var result = await AmazonS3.DeleteObject(input, connection, options, default);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(objects.Length, result.Data.Count);

            for (int i = 0; i < objects.Length; i++)
            {
                Assert.IsTrue(result.Data[i].Success);
                Assert.AreEqual(keys[i], result.Data[i].Key);
                Assert.IsNotNull(result.Data[i].VersionId);
                Assert.IsNull(result.Data[i].Error);
                Assert.IsFalse(FileExistsInS3(keys[i]).Result);
            }

            Assert.IsTrue(FileExistsInS3("ExampleFile").Result);

            CleanUp();
        }
    }

    [TestMethod]
    public async Task DeleteMultipleObjects_ReturnInfoOfNonExistingObject()
    {
        var keys = new[] { "Key1.txt", "Key2.txt", "Key3.txt" };
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = keys[0] }, new S3ObjectArray { BucketName = _bucketName, Key = keys[1] }, new S3ObjectArray { BucketName = _bucketName, Key = keys[2] } };

        await CreateTestFiles(objects);

        var inputSingle = new Input()
        {
            Objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = keys[0] } },
            ActionOnObjectNotFound = NotExistsHandler.Info,
        };

        var input = new Input()
        {
            Objects = objects,
            ActionOnObjectNotFound = NotExistsHandler.Info,
        };

        var connection = new Connection()
        {
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
        };

        var options = new Options()
        {
            Timeout = 1,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        // Delete one of the keys.
        var deleteSingle = await AmazonS3.DeleteObject(inputSingle, connection, options, default);
        Assert.IsTrue(deleteSingle.Success);
        Assert.AreEqual(1, deleteSingle.Data.Count);
        Assert.IsFalse(FileExistsInS3(keys[0]).Result);

        // Now the real test.
        var result = await AmazonS3.DeleteObject(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(objects.Length, result.Data.Count);

        for (int i = 0; i < objects.Length; i++)
        {
            if (result.Data[i].Key == keys[0])
            {
                Assert.IsFalse(result.Data[i].Success);
                Assert.AreEqual(keys[i], result.Data[i].Key);
                Assert.IsNull(result.Data[i].VersionId);
                Assert.IsTrue(result.Data[i].Error.Message.Contains("doesn't exist in"));
                Assert.IsFalse(FileExistsInS3(keys[i]).Result);
            }
            else
            {
                Assert.IsTrue(result.Data[i].Success);
                Assert.AreEqual(keys[i], result.Data[i].Key);
                Assert.IsNotNull(result.Data[i].VersionId);
                Assert.IsNull(result.Data[i].Error);
                Assert.IsFalse(FileExistsInS3(keys[i]).Result);
            }
        }

        Assert.IsTrue(FileExistsInS3("ExampleFile").Result);
    }

    [TestMethod]
    public async Task DeleteObject_NotExistsHandler_Throw()
    {
        var key = "Foo";
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key, VersionId = null } };

        var input = new Input()
        {
            Objects = objects,
            ActionOnObjectNotFound = NotExistsHandler.Throw,
        };

        var connection = new Connection()
        {
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
        };

        var options = new Options()
        {
            Timeout = 1,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.DeleteObject(input, connection, options, default));
        Assert.IsNotNull(ex.InnerException);
        Assert.IsTrue(ex.InnerException.Message.Contains("doesn't exist"));
    }

    [TestMethod]
    public async Task DeleteSingleObject_MissingInputs()
    {
        var key = "Foo";
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key, VersionId = null } };

        var input = new Input()
        {
            Objects = objects,
            ActionOnObjectNotFound = NotExistsHandler.None,
        };

        var connection = new Connection()
        {
            AwsAccessKeyId = "",
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
        };

        var options = new Options()
        {
            Timeout = 1,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<AmazonS3Exception>(async () => await AmazonS3.DeleteObject(input, connection, options, default));
        Assert.IsNotNull(ex.InnerException);
        Assert.AreEqual("Access Denied", ex.InnerException.Message);
    }

    [TestMethod]
    public async Task DeleteSingleObject_Missing_AwsSecretAccessKey()
    {
        var key = "Foo";
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key, VersionId = null } };

        var input = new Input()
        {
            Objects = objects,
            ActionOnObjectNotFound = NotExistsHandler.None,
        };
        
        var connection = new Connection()
        {
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = "",
            Region = Region.EuCentral1,
        };
        var options = new Options()
        {
            Timeout = 1,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.DeleteObject(input, connection, options, default));
        Assert.IsNotNull(ex.InnerException);
        Assert.IsTrue(ex.InnerException.Message.Contains("Value cannot be null"));
    }

    [TestMethod]
    public async Task DeleteSingleObject_Missing_Objects()
    {
        var input = new Input()
        {
            Objects = null,
            ActionOnObjectNotFound = NotExistsHandler.None,
        };

        var connection = new Connection()
        {
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
        };

        var options = new Options()
        {
            Timeout = 1,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.DeleteObject(input, connection, options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("DeleteObject error: Input.Objects cannot be empty.", ex.Message);
    }

    [TestMethod]
    public async Task DeleteSingleObject_ThrowErrorOnFailure_False()
    {
        var key = "Foo";
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key, VersionId = null } };

        var input = new Input()
        {
            Objects = objects,
            ActionOnObjectNotFound = NotExistsHandler.None,
        };

        var connection = new Connection()
        {
            AwsAccessKeyId = "",
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
        };

        var options = new Options()
        {
            Timeout = 1,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.DeleteObject(input, connection, options, default);
        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public async Task DeleteSingleObject_CustomErrorMessage()
    {
        var key = "Foo";
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key, VersionId = null } };

        var input = new Input()
        {
            Objects = objects,
            ActionOnObjectNotFound = NotExistsHandler.None,
        };

        var connection = new Connection()
        {
            AwsAccessKeyId = "",
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
        };

        var options = new Options()
        {
            Timeout = 1,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = "Custom error message"
        };

        var ex = await Assert.ThrowsExceptionAsync<AmazonS3Exception>(async () => await AmazonS3.DeleteObject(input, connection, options, default));
        Assert.AreEqual("Custom error message", ex.Message);
    }

    private async Task CreateTestFiles(S3ObjectArray[] array)
    {
        try
        {
            Directory.CreateDirectory($@"{_dir}\TempFiles");
            using var client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);

            foreach (var item in array)
            {
                var filePath = $@"{_dir}\TempFiles\{item.Key}";
                File.AppendAllText(filePath, $"{item.Key}");
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = item.BucketName,
                    Key = item.Key,
                    FilePath = filePath,
                };
                await client.PutObjectAsync(putObjectRequest);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private async Task<bool> FileExistsInS3(string key)
    {
        using var client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);

        var request = new ListObjectsRequest
        {
            BucketName = _bucketName,
            Prefix = key,
        };
        ListObjectsResponse response = await client.ListObjectsAsync(request);
        return (response != null && response.S3Objects != null && response.S3Objects.Count > 0);
    }
}
