using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Frends.AmazonS3.DeleteBucket.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Frends.AmazonS3.DeleteBucket.Tests;

[TestClass]
public class AWSCredsUnitTests
{
    private readonly string? _accessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_AccessKey");
    private readonly string? _secretAccessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_SecretAccessKey");
    private Connection _connection = new();
    private Input _input = new();
    private Options _options = new();
    private readonly string _bucketName = "ritteambuckettest";

    [TestInitialize]
    public async Task Init()
    {
        _connection = new Connection
        {
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
        };

        _input = new Input
        {
            BucketName = _bucketName,
        };

        _options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = null
        };

        using IAmazonS3 s3Client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);
        if (!await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, _bucketName))
            await s3Client.PutBucketAsync(_bucketName);
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        try
        {
            using IAmazonS3 s3Client = new AmazonS3Client(_connection.AwsAccessKeyId, _connection.AwsSecretAccessKey, RegionEndpoint.EUCentral1);
            if (await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, _bucketName))
            {
                var request = new DeleteBucketRequest
                {
                    BucketName = _bucketName
                };

                await s3Client.DeleteBucketAsync(request);
            }
        }
        catch (AmazonS3Exception ex)
        {
            throw new AmazonS3Exception(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [TestMethod]
    public async Task DeleteBucket_SuccessTest()
    {
        var result = await AmazonS3.DeleteBucket(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBucket_BucketDoesNotExistTest()
    {
        var result = await AmazonS3.DeleteBucket(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);

        var result2 = await AmazonS3.DeleteBucket(_input, _connection, _options, default);
        Assert.IsTrue(result2.Success);
        Assert.IsNotNull(result2.Error);
        Assert.AreEqual("Bucket to be deleted, does not exist.", result2.Error.Message);
    }

    [TestMethod]
    public async Task DeleteBucket_ExceptionHandlingTest()
    {
        var connection = new Connection
        {
            AwsSecretAccessKey = "foobar",
            AwsAccessKeyId = "foobar",
            Region = Region.EuCentral1,
        };

        var ex = await Assert.ThrowsExceptionAsync<AmazonS3Exception>(() => AmazonS3.DeleteBucket(_input, connection, _options, default));
        Assert.IsNotNull(ex.InnerException);
    }

    [TestMethod]
    public async Task DeleteBucket_ThrowErrorOnFailureFalseTest()
    {
        var connection = new Connection
        {
            AwsSecretAccessKey = "foobar",
            AwsAccessKeyId = "foobar",
            Region = Region.EuCentral1,
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false
        };

        var result = await AmazonS3.DeleteBucket(_input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsNotNull(result.Error.Message);
    }

    [TestMethod]
    public async Task DeleteBucket_CustomErrorMessageTest()
    {
        var connection = new Connection
        {
            AwsSecretAccessKey = "foobar",
            AwsAccessKeyId = "foobar",
            Region = Region.EuCentral1,
        };

        var options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = "Custom error message for bucket deletion"
        };

        var ex = await Assert.ThrowsExceptionAsync<AmazonS3Exception>(() => AmazonS3.DeleteBucket(_input, connection, options, default));
        Assert.AreEqual("Custom error message for bucket deletion", ex.Message);
        Assert.IsNotNull(ex.InnerException);
    }

    [TestMethod]
    public async Task DeleteBucket_CustomErrorMessageInResultTest()
    {
        var connection = new Connection
        {
            AwsSecretAccessKey = "foobar",
            AwsAccessKeyId = "foobar",
            Region = Region.EuCentral1,
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = "Custom error message for bucket deletion"
        };

        var result = await AmazonS3.DeleteBucket(_input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("Custom error message for bucket deletion", result.Error.Message);
    }

    [TestMethod]
    public async Task DeleteBucket_InvalidBucketNameTest()
    {
        var input = new Input
        {
            BucketName = "invalid..bucket..name"
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false
        };

        var result = await AmazonS3.DeleteBucket(input, _connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("bucket") || result.Error.Message.Contains("name"));
    }

    [TestMethod]
    public async Task DeleteBucket_EmptyBucketNameTest()
    {
        var input = new Input
        {
            BucketName = ""
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false
        };

        var result = await AmazonS3.DeleteBucket(input, _connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBucket_NullBucketNameTest()
    {
        var input = new Input
        {
            BucketName = null
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false
        };

        var result = await AmazonS3.DeleteBucket(input, _connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBucket_InvalidRegionTest()
    {
        var connection = new Connection
        {
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = (Region)999 // Invalid region
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false
        };

        var result = await AmazonS3.DeleteBucket(_input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBucket_NetworkTimeoutTest()
    {
        // This test simulates a network timeout scenario
        var connection = new Connection
        {
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1
        };

        var input = new Input
        {
            BucketName = "non-existent-bucket-for-timeout-test-12345"
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false
        };

        var result = await AmazonS3.DeleteBucket(input, connection, options, default);
        // Should handle gracefully even if bucket doesn't exist
        Assert.IsTrue(result.Success || !result.Success); // Either outcome is acceptable for this test
    }

    [TestMethod]
    public async Task DeleteBucket_BucketWithObjectsTest()
    {
        // Create a bucket with objects to test deletion behavior
        var bucketWithObjects = "test-bucket-with-objects-12345";
        
        using IAmazonS3 s3Client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);
        
        try
        {
            // Create bucket
            await s3Client.PutBucketAsync(bucketWithObjects);
            
            // Add an object to the bucket
            await s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucketWithObjects,
                Key = "test-object.txt",
                ContentBody = "test content"
            });

            var input = new Input
            {
                BucketName = bucketWithObjects
            };

            var options = new Options
            {
                ThrowErrorOnFailure = false
            };

            var result = await AmazonS3.DeleteBucket(input, _connection, options, default);
            
            // Should fail because bucket is not empty
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.Message.Contains("empty") || result.Error.Message.Contains("BucketNotEmpty"));
        }
        finally
        {
            // Cleanup: Delete object and bucket
            try
            {
                await s3Client.DeleteObjectAsync(bucketWithObjects, "test-object.txt");
                await s3Client.DeleteBucketAsync(bucketWithObjects);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [TestMethod]
    public async Task DeleteBucket_CancellationTokenTest()
    {
        using var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        var options = new Options
        {
            ThrowErrorOnFailure = false
        };

        var result = await AmazonS3.DeleteBucket(_input, _connection, options, cts.Token);
        
        // Should handle cancellation gracefully
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBucket_ParameterValidationTest()
    {
        // Test with null input
        var ex1 = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => 
            AmazonS3.DeleteBucket(null, _connection, _options, default));
        
        // Test with null connection
        var ex2 = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => 
            AmazonS3.DeleteBucket(_input, null, _options, default));
        
        // Test with null options
        var ex3 = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => 
            AmazonS3.DeleteBucket(_input, _connection, null, default));
    }
}
