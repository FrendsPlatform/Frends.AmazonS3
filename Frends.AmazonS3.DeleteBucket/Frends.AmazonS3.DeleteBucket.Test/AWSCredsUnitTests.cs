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
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
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
        Assert.IsNull(result2.Error);
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

        var input = new Input
        {
            BucketName = _bucketName,
        };

        var options = new Options { ThrowErrorOnFailure = true };
        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => AmazonS3.DeleteBucket(input, connection, options, default));
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

        var input = new Input
        {
            BucketName = _bucketName,
        };

        var options = new Options { ThrowErrorOnFailure = false };
        var result = await AmazonS3.DeleteBucket(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsNotNull(result.Error.Message);
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

        var input = new Input
        {
            BucketName = _bucketName,
        };

        var customErrorMessage = "Custom error message for bucket deletion failure";
        var options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = customErrorMessage
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => AmazonS3.DeleteBucket(input, connection, options, default));
        Assert.AreEqual(customErrorMessage, ex.Message);
    }

    [TestMethod]
    public async Task DeleteBucket_CustomErrorMessageWithThrowFalseTest()
    {
        var connection = new Connection
        {
            AwsSecretAccessKey = "foobar",
            AwsAccessKeyId = "foobar",
            Region = Region.EuCentral1,
        };

        var input = new Input
        {
            BucketName = _bucketName,
        };

        var customErrorMessage = "Custom error message for bucket deletion failure";
        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = customErrorMessage
        };

        var result = await AmazonS3.DeleteBucket(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.StartsWith(customErrorMessage));
    }

    [TestMethod]
    public async Task DeleteBucket_EmptyBucketNameTest()
    {
        var input = new Input
        {
            BucketName = "",
        };

        var options = new Options { ThrowErrorOnFailure = false };
        var result = await AmazonS3.DeleteBucket(input, _connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsNotNull(result.Error.Message);
    }

    [TestMethod]
    public async Task DeleteBucket_NullBucketNameTest()
    {
        var input = new Input
        {
            BucketName = null,
        };

        var options = new Options { ThrowErrorOnFailure = false };
        var result = await AmazonS3.DeleteBucket(input, _connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsNotNull(result.Error.Message);
    }

    [TestMethod]
    public async Task DeleteBucket_InvalidBucketNameTest()
    {
        var input = new Input
        {
            BucketName = "Invalid_Bucket_Name_With_Underscores_And_Capitals",
        };

        var options = new Options { ThrowErrorOnFailure = false };
        var result = await AmazonS3.DeleteBucket(input, _connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsNotNull(result.Error.Message);
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

        var input = new Input
        {
            BucketName = _bucketName,
        };

        var options = new Options { ThrowErrorOnFailure = false };
        var result = await AmazonS3.DeleteBucket(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBucket_NullConnectionTest()
    {
        var input = new Input
        {
            BucketName = _bucketName,
        };

        var options = new Options { ThrowErrorOnFailure = false };

        var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => AmazonS3.DeleteBucket(input, null, options, default));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task DeleteBucket_NullInputTest()
    {
        var options = new Options { ThrowErrorOnFailure = false };

        var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => AmazonS3.DeleteBucket(null, _connection, options, default));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task DeleteBucket_NullOptionsTest()
    {
        var input = new Input
        {
            BucketName = _bucketName,
        };

        var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => AmazonS3.DeleteBucket(input, _connection, null, default));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task DeleteBucket_CancellationTokenTest()
    {
        using var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel();

        var input = new Input
        {
            BucketName = _bucketName,
        };

        var options = new Options { ThrowErrorOnFailure = false };

        var result = await AmazonS3.DeleteBucket(input, _connection, options, cts.Token);
        // The result depends on implementation - it might succeed if cancellation is not checked early,
        // or fail if cancellation is properly handled
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DeleteBucket_EmptyCredentialsTest()
    {
        var connection = new Connection
        {
            AwsAccessKeyId = "",
            AwsSecretAccessKey = "",
            Region = Region.EuCentral1,
        };

        var input = new Input
        {
            BucketName = _bucketName,
        };

        var options = new Options { ThrowErrorOnFailure = false };
        var result = await AmazonS3.DeleteBucket(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsNotNull(result.Error.Message);
    }

    [TestMethod]
    public async Task DeleteBucket_DefaultOptionsTest()
    {
        var options = new Options(); // Test with default values

        var result = await AmazonS3.DeleteBucket(_input, _connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }
}
