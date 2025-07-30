using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Frends.AmazonS3.CreateBucket.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Frends.AmazonS3.CreateBucket.Tests;

[TestClass]
public class AWSCredsUnitTests
{
    public TestContext? TestContext { get; set; }
    private readonly string? _accessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_AccessKey");
    private readonly string? _secretAccessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_SecretAccessKey");
    private Connection _connection = new();
    private Options _options = new();
    private Input _input = new();
    private string? _bucketName;

    [TestInitialize]
    public void Init()
    {
        _connection = new Connection
        {
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
        };

        _options = new Options
        {
            ObjectLockEnabled = false,
            ThrowErrorOnFailure = true,
        };
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
    public async Task CreateBucket_SuccessTest()
    {
        var acl = Acls.Private;
        _bucketName = $"ritteambuckettest{acl.ToString().ToLower()}";
        _input.BucketName = _bucketName;
        _connection.Acl = acl;

        var result = await AmazonS3.CreateBucket(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual("eu-central-1", result.BucketLocation);
    }

    [TestMethod]
    public async Task CreateBucket_BucketAlreadyExistsTest()
    {
        var acl = Acls.Private;
        _bucketName = $"ritteambuckettest{acl.ToString().ToLower()}";
        _input.BucketName = _bucketName;
        _connection.Acl = acl;

        var result = await AmazonS3.CreateBucket(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual("eu-central-1", result.BucketLocation);

        var result2 = await AmazonS3.CreateBucket(_input, _connection, _options, default);
        Assert.IsTrue(result2.Success);
        Assert.AreEqual("Bucket already exists.", result2.BucketLocation);
    }

    [TestMethod]
    public async Task CreateBucket_ExceptionHandlingTest()
    {
        var acl = Acls.PublicRead;
        _bucketName = $"ritteambuckettest{acl.ToString().ToLower()}";
        _input.BucketName = _bucketName;
        _connection.Acl = acl;

        var ex = await Assert.ThrowsExceptionAsync<AmazonS3Exception>(() => AmazonS3.CreateBucket(_input, _connection, _options, default));
        Assert.IsNotNull(ex.InnerException);
        Assert.AreEqual("Access Denied", ex.InnerException.Message);
    }
}