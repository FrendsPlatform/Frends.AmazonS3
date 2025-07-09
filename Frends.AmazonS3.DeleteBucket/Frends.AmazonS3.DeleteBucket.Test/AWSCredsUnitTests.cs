using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Frends.AmazonS3.DeleteBucket.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
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
        Assert.IsNull(result2.Error.Message);
        Assert.AreEqual("Bucket to be deleted, does not exist.", result2.Error.AdditionalInfo);
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
    public async Task DeleteBucket_InvalidRegionTest()
    {
        var connection = new Connection
        {
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = (Region)999, // Invalid region
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = null
        };

        var result = await AmazonS3.DeleteBucket(_input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBucket_EmptyBucketNameTest()
    {
        var input = new Input
        {
            BucketName = string.Empty,
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = null
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
            BucketName = null,
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = null
        };

        var result = await AmazonS3.DeleteBucket(input, _connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBucket_InvalidBucketNameTest()
    {
        var input = new Input
        {
            BucketName = "Invalid_Bucket_Name_With_Underscores_And_Too_Long_Name_That_Exceeds_Limits",
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = null
        };

        var result = await AmazonS3.DeleteBucket(input, _connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBucket_NullConnectionTest()
    {
        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = null
        };

        var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => AmazonS3.DeleteBucket(_input, null, options, default));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task DeleteBucket_NullOptionsTest()
    {
        var result = await AmazonS3.DeleteBucket(_input, _connection, null, default);
        // Should handle null options gracefully
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DeleteBucket_CancellationTokenTest()
    {
        using var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel();

        var ex = await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => AmazonS3.DeleteBucket(_input, _connection, _options, cts.Token));
        Assert.IsNotNull(ex);
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
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = null
        };

        var result = await AmazonS3.DeleteBucket(_input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("Failed to delete the bucket", result.Error.Message);
        Assert.IsNotNull(result.Error.AdditionalInfo);
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

        var customErrorMessage = "Custom error occurred during bucket deletion";
        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = customErrorMessage
        };

        var result = await AmazonS3.DeleteBucket(_input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual(customErrorMessage, result.Error.Message);
        Assert.IsNotNull(result.Error.AdditionalInfo);
    }
}
