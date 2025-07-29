using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Amazon.S3;
using Amazon;
using Amazon.S3.Model;
using Frends.AmazonS3.DeleteObject.Definitions;
using Frends.AmazonS3.DeleteObject.Helpers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.S3.Util;
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
            Assert.AreEqual(objects.Length, result.DeletedObjects.Count);
            Assert.AreEqual(_bucketName, result.DeletedObjects[0].BucketName);
            Assert.AreEqual(key, result.DeletedObjects[0].Key);
            Assert.IsNotNull(result.DeletedObjects[0].VersionId);
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
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key } };
        var handlers = new List<NotExistsHandler>() { NotExistsHandler.None, NotExistsHandler.Info, NotExistsHandler.Throw };

        foreach (var handler in handlers)
        {
            var createdObjects = await CreateTestFiles(objects);
            var input = new Input()
            {
                Objects = createdObjects,
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
            using var client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);
            var versioningRequest = new GetBucketVersioningRequest { BucketName = _bucketName };
            var versioningResponse = await client.GetBucketVersioningAsync(versioningRequest);
            Assert.AreEqual(VersionStatus.Enabled, versioningResponse.VersioningConfig.Status, "Bucket versioning must be enabled for this test");
            var result = await AmazonS3.DeleteObject(input, connection, options, default);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(createdObjects.Length, result.DeletedObjects.Count);
            Assert.AreEqual(_bucketName, result.DeletedObjects[0].BucketName);
            Assert.AreEqual(key, result.DeletedObjects[0].Key);
            Assert.IsNotNull(result.DeletedObjects[0].VersionId);
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
            Assert.AreEqual(objects.Length, result.DeletedObjects.Count);

            for (int i = 0; i < objects.Length; i++)
            {
                Assert.AreEqual(_bucketName, result.DeletedObjects[i].BucketName);
                Assert.AreEqual(keys[i], result.DeletedObjects[i].Key);
                Assert.IsNotNull(result.DeletedObjects[i].VersionId);
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
        Assert.AreEqual(1, deleteSingle.DeletedObjects.Count);
        Assert.IsFalse(FileExistsInS3(keys[0]).Result);

        // Now the real test.
        var result = await AmazonS3.DeleteObject(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(objects.Length, result.DeletedObjects.Count);

        for (int i = 0; i < objects.Length; i++)
        {
            Assert.AreEqual(_bucketName, result.DeletedObjects[i].BucketName);
            Assert.AreEqual(keys[i], result.DeletedObjects[i].Key);
            if (result.DeletedObjects[i].Key == keys[0])
            {
                Assert.IsNull(result.DeletedObjects[i].VersionId);
            }
            else
            {
                Assert.IsNotNull(result.DeletedObjects[i].VersionId);
            }
            Assert.IsFalse(FileExistsInS3(keys[i]).Result);
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
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = ""
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.DeleteObject(input, connection, options, default));
        Assert.IsTrue(ex.Message.Contains("doesn't exist"));
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

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.DeleteObject(input, connection, options, default));
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
        Assert.IsTrue(ex.Message.Contains("Value cannot be null"));
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
        Assert.IsNotNull(result.Error);
        Assert.IsNotNull(result.Error.Message);
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

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.DeleteObject(input, connection, options, default));
        Assert.IsTrue(ex.Message.Contains("Custom error message"));
    }

    [TestMethod]
    public void ErrorHandler_Handle_AmazonS3Exception_ThrowOnFailure()
    {
        // Arrange
        var deletedObjects = new List<SingleResultObject>();
        var exception = new AmazonS3Exception("Test S3 exception");

        // Act & Assert
        var ex = Assert.ThrowsException<AmazonS3Exception>(() =>
            ErrorHandler.Handle(exception, true, "", deletedObjects));
        Assert.IsTrue(ex.Message.Contains("Test S3 exception"));
    }

    [TestMethod]
    public void ErrorHandler_Handle_GenericException_ThrowOnFailure()
    {
        // Arrange
        var deletedObjects = new List<SingleResultObject>();
        var exception = new Exception("Test generic exception");

        // Act & Assert
        var ex = Assert.ThrowsException<Exception>(() =>
            ErrorHandler.Handle(exception, true, "", deletedObjects));
        Assert.IsTrue(ex.Message.Contains("Test generic exception"));
    }

    [TestMethod]
    public async Task DeleteMultipleObjects_PartialFailure()
    {
        var keys = new[] { "Key1.txt", "NonExistentKey.txt", "Key3.txt" };
        var objects = new[] {
            new S3ObjectArray { BucketName = _bucketName, Key = keys[0] },
            new S3ObjectArray { BucketName = "non-existent-bucket", Key = keys[1] },
            new S3ObjectArray { BucketName = _bucketName, Key = keys[2] }
        };

        // Create only the first and third objects
        var validObjects = new[] {
            new S3ObjectArray { BucketName = _bucketName, Key = keys[0] },
            new S3ObjectArray { BucketName = _bucketName, Key = keys[2] }
        };
        await CreateTestFiles(validObjects);

        var input = new Input()
        {
            Objects = objects,
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
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        var result = await AmazonS3.DeleteObject(input, connection, options, default);

        // Should return false due to partial failure, but still contain successfully deleted objects
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.DeletedObjects.Count > 0); // Some objects should have been deleted successfully
    }

    private async Task<S3ObjectArray[]> CreateTestFiles(S3ObjectArray[] array)
    {
        try
        {
            Directory.CreateDirectory($@"{_dir}\TempFiles");
            using var client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);

            var createdObjects = new List<S3ObjectArray>();

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
                var response = await client.PutObjectAsync(putObjectRequest);
                createdObjects.Add(new S3ObjectArray
                {
                    BucketName = item.BucketName,
                    Key = item.Key,
                    VersionId = response.VersionId
                });
            }
            return createdObjects.ToArray();
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

    [TestMethod]
    public void Result_Constructor_Success()
    {
        // Arrange
        var deletedObjects = new List<SingleResultObject>
        {
            new SingleResultObject { BucketName = "test-bucket", Key = "test-key", VersionId = "v1" }
        };

        // Act
        var result = new Result(true, deletedObjects, null);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.DeletedObjects.Count);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public void Result_Constructor_WithError()
    {
        // Arrange
        var deletedObjects = new List<SingleResultObject>();
        var error = new Error
        {
            Message = "Test error message",
            AdditionalInfo = new List<SingleResultObject>()
        };

        // Act
        var result = new Result(false, deletedObjects, error);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(0, result.DeletedObjects.Count);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("Test error message", result.Error.Message);
    }

    [TestMethod]
    public async Task DeleteObject_EmptyBucketName_ThrowsException()
    {
        // Arrange
        var objects = new[] { new S3ObjectArray { BucketName = "", Key = "test-key", VersionId = null } };

        var input = new Input()
        {
            Objects = objects,
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

        // Act & Assert
        var ex = await Assert.ThrowsExceptionAsync<Exception>(
            async () => await AmazonS3.DeleteObject(input, connection, options, default));
        Assert.IsTrue(ex.Message.Contains("BucketName cannot be null or empty"));
    }

    [TestMethod]
    public async Task DeleteObject_EmptyKey_ThrowsException()
    {
        // Arrange
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = "", VersionId = null } };

        var input = new Input()
        {
            Objects = objects,
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

        // Act & Assert
        var ex = await Assert.ThrowsExceptionAsync<Exception>(
            async () => await AmazonS3.DeleteObject(input, connection, options, default));
        Assert.IsTrue(ex.Message.Contains("Object Key cannot be null or empty"));
    }

    [TestMethod]
    public async Task DeleteObject_PartialFailure_CustomErrorMessage()
    {
        // Arrange
        var keys = new[] { "Key1.txt", "Key2.txt" };
        var objects = new[] {
            new S3ObjectArray { BucketName = _bucketName, Key = keys[0] },
            new S3ObjectArray { BucketName = "non-existent-bucket", Key = keys[1] }
        };

        // Create only the first object
        var validObjects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = keys[0] } };
        await CreateTestFiles(validObjects);

        var customErrorMessage = "Custom partial failure message";
        var input = new Input()
        {
            Objects = objects,
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
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = customErrorMessage
        };

        // Act
        var result = await AmazonS3.DeleteObject(input, connection, options, default);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains(customErrorMessage));
        Assert.IsTrue(result.DeletedObjects.Count > 0); // Some objects should have been deleted successfully
    }

    [TestMethod]
    public async Task DeleteObject_ZeroTimeout_HandlesGracefully()
    {
        // Arrange
        var key = "Key1.txt";
        var objects = new[] { new S3ObjectArray { BucketName = _bucketName, Key = key, VersionId = null } };
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
            Region = Region.EuCentral1,
        };

        var options = new Options()
        {
            Timeout = 0, // Zero timeout
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        // Act
        var result = await AmazonS3.DeleteObject(input, connection, options, default);

        // Assert - Should handle zero timeout gracefully
        // Result may succeed or fail depending on AWS response time, but shouldn't crash
        Assert.IsNotNull(result);
    }
}
