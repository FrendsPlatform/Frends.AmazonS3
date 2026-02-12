using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using NUnit.Framework;

namespace Frends.AmazonS3.ListObjectVersions.Tests;

[TestFixture]
public class FunctionalTests : TestBase
{
    private const string FirstTestFile = "test_file1.txt";

    private readonly string[] testFiles =
    [
        FirstTestFile,
        "test_folder/test_file2.txt",
        "2020/11/23/file3.txt",
        "test_folder/subfolder/20220401.txt",
    ];

    private AmazonS3Client s3Client;

    [OneTimeSetUp]
    public async Task Setup()
    {
        s3Client = new AmazonS3Client(AccessKey, SecretAccessKey, RegionEndpoint.EUCentral1);

        if (!await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, BucketName))
        {
            await s3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = BucketName,
                BucketRegion = S3Region.EUCentral1,
            });
            await Task.Delay(1000);
            await SetVersioning(true);

            foreach (var file in testFiles)
            {
                await UploadObject(file);
            }
        }

        await UploadObject(FirstTestFile, "Updated content");
        await Task.Delay(1000);
    }

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        try
        {
            if (s3Client != null && await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, BucketName))
            {
                var listResponse = await s3Client.ListVersionsAsync(new ListVersionsRequest
                {
                    BucketName = BucketName,
                });

                var objectsToDelete = listResponse.Versions
                    .Select(v => new KeyVersion
                    {
                        Key = v.Key,
                        VersionId = v.VersionId,
                    })
                    .ToList();

                if (objectsToDelete.Count != 0)
                {
                    var deleteRequest = new DeleteObjectsRequest
                    {
                        BucketName = BucketName,
                        Objects = objectsToDelete,
                    };
                    await s3Client.DeleteObjectsAsync(deleteRequest);
                }

                await Task.Delay(1000);
                await s3Client.DeleteBucketAsync(BucketName);

                await Task.Delay(1000);
                bool bucketStillExists = await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, BucketName);

                if (bucketStillExists)
                {
                    throw new Exception("Bucket still exists after deletion attempt. Needs to be deleted manually.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cleanup failed: {ex.Message}");
        }
        finally
        {
            s3Client?.Dispose();
        }
    }

    [Test]
    public async Task ListAllObjectsVersions()
    {
        var result = await AmazonS3.ListObjectVersions(
            DefaultInput(),
            DefaultConnection(),
            DefaultOptions(),
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Objects, Is.Not.Null);
        Assert.That(result.Objects.Count, Is.EqualTo(testFiles.Length + 1));
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task ListSelectedObjectVersions()
    {
        var opt = DefaultOptions();
        opt.Prefix = FirstTestFile;
        var result = await AmazonS3.ListObjectVersions(
            DefaultInput(),
            DefaultConnection(),
            opt,
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Objects, Is.Not.Null);
        Assert.That(result.Objects.Count, Is.EqualTo(2));
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task ListLimitedByMaxKeys()
    {
        var opt = DefaultOptions();
        opt.MaxKeys = 2;
        var result = await AmazonS3.ListObjectVersions(
            DefaultInput(),
            DefaultConnection(),
            opt,
            CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Objects, Is.Not.Null);
        Assert.That(result.Objects.Count, Is.EqualTo(testFiles.Length + 1));
        Assert.That(result.Error, Is.Null);
    }

    private async Task UploadObject(string fileKey, string content = "test content")
    {
        await s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = BucketName,
            Key = fileKey,
            ContentBody = content,
        });
    }

    private async Task SetVersioning(bool enabled)
    {
        await s3Client.PutBucketVersioningAsync(new PutBucketVersioningRequest
        {
            BucketName = BucketName,
            VersioningConfig = new S3BucketVersioningConfig
            {
                Status = enabled ? VersionStatus.Enabled : VersionStatus.Suspended,
            },
        });
    }
}
