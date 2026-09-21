using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frends.AmazonS3.UploadObject.Tests;

[TestClass]
public class TestAssemblySetup : AwsS3TestBase
{
    private static AmazonS3Client? s3Client;

    [AssemblyInitialize]
    public static async Task Initialize(TestContext _)
    {

        if (string.IsNullOrWhiteSpace(AccessKey) || string.IsNullOrWhiteSpace(SecretAccessKey))
            throw new InvalidOperationException("AWS test credentials are required to run UploadObject tests.");

        s3Client = new AmazonS3Client(AccessKey, SecretAccessKey, RegionEndpoint.EUCentral1);
        await CleanupStaleBucketsAsync();

        await s3Client.PutBucketAsync(new PutBucketRequest
        {
            BucketName = BucketName,
            ObjectOwnership = ObjectOwnership.ObjectWriter,
            UseClientRegion = true,
        });
    }

    [AssemblyCleanup]
    public static async Task Cleanup()
    {
        if (s3Client is null || string.IsNullOrWhiteSpace(BucketName))
            return;

        try
        {
            await AbortMultipartUploadsAsync(BucketName);
            await DeleteObjectsAsync(BucketName);
            await s3Client.DeleteBucketAsync(BucketName);
        }
        finally
        {
            s3Client.Dispose();
        }
    }

    private static async Task DeleteObjectsAsync(string bucket)
    {
        string? continuationToken = null;

        do
        {
            var response = await s3Client!.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = bucket,
                ContinuationToken = continuationToken,
            });

            if (response.S3Objects is not null && response.S3Objects.Count > 0)
            {
                var objects = new List<KeyVersion>(response.S3Objects.Count);
                foreach (var item in response.S3Objects)
                    objects.Add(new KeyVersion
                    {
                        Key = item.Key,
                    });

                await s3Client.DeleteObjectsAsync(new DeleteObjectsRequest
                {
                    BucketName = bucket,
                    Objects = objects,
                });
            }

            continuationToken = response.IsTruncated == true ? response.NextContinuationToken : null;
        } while (continuationToken is not null);
    }

    private static async Task AbortMultipartUploadsAsync(string bucket)
    {
        string? keyMarker = null;
        string? uploadIdMarker = null;

        do
        {
            var response = await s3Client!.ListMultipartUploadsAsync(new ListMultipartUploadsRequest
            {
                BucketName = bucket,
                KeyMarker = keyMarker,
                UploadIdMarker = uploadIdMarker,
            });

            if (response.MultipartUploads is not null)
            {
                foreach (var upload in response.MultipartUploads)
                {
                    await s3Client.AbortMultipartUploadAsync(new AbortMultipartUploadRequest
                    {
                        BucketName = bucket,
                        Key = upload.Key,
                        UploadId = upload.UploadId,
                    });
                }
            }

            keyMarker = response.IsTruncated == true ? response.NextKeyMarker : null;
            uploadIdMarker = response.IsTruncated == true ? response.NextUploadIdMarker : null;
        } while (keyMarker is not null || uploadIdMarker is not null);
    }

    private static async Task CleanupStaleBucketsAsync()
    {
        var response = await s3Client!.ListBucketsAsync();

        foreach (var bucket in response.Buckets)
        {
            if (!bucket.BucketName.StartsWith(BucketPrefix, StringComparison.Ordinal))
                continue;

            await AbortMultipartUploadsAsync(bucket.BucketName);
            await DeleteObjectsAsync(bucket.BucketName);
            await s3Client.DeleteBucketAsync(bucket.BucketName);
        }
    }
}
