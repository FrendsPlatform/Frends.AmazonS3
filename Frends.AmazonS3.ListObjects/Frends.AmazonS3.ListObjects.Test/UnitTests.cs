using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Frends.AmazonS3.ListObjects.Definitions;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using System.Threading.Tasks;

namespace Frends.AmazonS3.ListObjects.Test
{
    [TestClass]
    public class ListObjectsTest
    {
        private readonly string? _accessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_AccessKey");
        private readonly string? _secretAccessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_SecretAccessKey");
        private readonly string? _bucketName = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_BucketName");

        Source? _source = null;
        Options? _options = null;

        private AmazonS3Client _client = null!;

        [TestInitialize]
        public async Task Setup()
        {
            _client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);

            await _client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = "testfolder/subfolder/20220402.txt",
                ContentBody = "Test file for StartAfter"
            });

            await _client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = "2020/11/23/testfile.txt",
                ContentBody = "Test file for Prefix"
            });

            for (int i = 0; i < 5; i++)
            {
                await _client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"pagetest/file_{i}.txt",
                    ContentBody = $"Pagination test file {i}"
                });
            }

            for (int i = 0; i < 3; i++)
            {
                await _client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"testfolder/file_{i}.txt",
                    ContentBody = $"Delimiter test file {i}"
                });
            }
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            var keysToDelete = new[]
            {
                "testfolder/subfolder/20220402.txt",
                "2020/11/23/testfile.txt",
                "testfolder/file_0.txt",
                "testfolder/file_1.txt",
                "testfolder/file_2.txt",
                "pagetest/file_0.txt",
                "pagetest/file_1.txt",
                "pagetest/file_2.txt",
                "pagetest/file_3.txt",
                "pagetest/file_4.txt"
            };

            foreach (var key in keysToDelete)
            {
                await _client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                });
            }
        }


        /// <summary>
        /// List all objects.
        /// </summary>
        [TestMethod]
        public void ListAllTest()
        {
            _source = new Source
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                BucketName = _bucketName,
                Region = Region.EuCentral1
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 1000,
                Prefix = null,
                StartAfter = null
            };

            var result = AmazonS3.ListObjects(_source, _options, default);
            Assert.IsNotNull(result.Result.ObjectList);
        }

        /// <summary>
        /// With StartAfter value. Get objects created after 2022-04-22T00:16:40+02:00.
        /// </summary>
        [TestMethod]
        public void UsingStartAfterTest()
        {
            _source = new Source
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                BucketName = _bucketName,
                Region = Region.EuCentral1
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 100,
                Prefix = null,
                StartAfter = "testfolder/subfolder/20220401.txt"
            };

            var result = AmazonS3.ListObjects(_source, _options, default);
            Assert.IsNotNull(result.Result.ObjectList);
        }

        /// <summary>
        /// With Prefix value. Get objects from 2020/11/23/ locations.
        /// </summary>
        [TestMethod]
        public void UsingPrefixTest()
        {
            _source = new Source
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                BucketName = _bucketName,
                Region = Region.EuCentral1
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 100,
                Prefix = "2020/11/23/",
                StartAfter = null
            };

            var result = AmazonS3.ListObjects(_source, _options, default);
            Assert.IsNotNull(result.Result.ObjectList);
        }

        /// <summary>
        /// Missing AwsAccessKeyId. Authentication fail.
        /// </summary>
        [TestMethod]
        public void MissingKeyTest()
        {
            _source = new Source
            {
                AwsAccessKeyId = null,
                AwsSecretAccessKey = _secretAccessKey,
                BucketName = _bucketName,
                Region = Region.EuCentral1
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 100,
                Prefix = null,
                StartAfter = null
            };

            var ex = Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.ListObjects(_source, _options, default)).Result;
            Assert.IsTrue(ex.Message.Contains("AWS credentials missing."));
        }

        [TestMethod]
        public void InvalidBucketName_ShouldThrowAmazonS3Exception()
        {
            var invalidSource = new Source
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                BucketName = "invalid-bucket-name",
                Region = Region.EuCentral1
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 1000,
                Prefix = null,
                StartAfter = null
            };

            var ex = Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.ListObjects(invalidSource, _options, default)).Result;

            Assert.IsTrue(ex.Message.Contains("The bucket you are attempting to access must be addressed using the specified endpoint."));
        }

        public void UnsupportedRegion_ShouldThrowInvalidOperationException()
        {
            var invalidSource = new Source
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                BucketName = _bucketName,
                Region = (Region)999 
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 1000,
                Prefix = null,
                StartAfter = null
            };

            var exception = Assert.ThrowsException<AggregateException>(() =>
                AmazonS3.ListObjects(invalidSource, _options, default).Result);

            Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
        }


        [TestMethod]
        public async Task DelimiterUsageTest()
        {
            _source = new Source
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                BucketName = _bucketName,
                Region = Region.EuCentral1
            };

            _options = new Options
            {
                Delimiter = "/",
                MaxKeys = 100,
                Prefix = "testfolder/",
                StartAfter = null
            };

            var result = await AmazonS3.ListObjects(_source, _options, default);
            Assert.IsTrue(result.ObjectList.Count > 0);
        }

        [TestMethod]
        public void PaginationTest_ShouldRespectMaxKeys()
        {
            _source = new Source
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                BucketName = _bucketName,
                Region = Region.EuCentral1
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 3,
                Prefix = "pagetest/",
                StartAfter = null
            };

            var result = AmazonS3.ListObjects(_source, _options, default).Result;

            Assert.AreEqual(3, result.ObjectList.Count);

            foreach (var obj in result.ObjectList)
            {
                Assert.IsTrue(obj.Key.StartsWith("pagetest/"));
            }
        }

    }
}