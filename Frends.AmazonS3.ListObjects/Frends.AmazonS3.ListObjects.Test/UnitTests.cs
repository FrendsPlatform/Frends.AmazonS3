using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Frends.AmazonS3.ListObjects.Definitions;

namespace Frends.AmazonS3.ListObjects.Test
{
    [TestClass]
    public class ListObjectsTest
    {
        private readonly string? _accessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_AccessKey");
        private readonly string? _secretAccessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_SecretAccessKey");
        private readonly string? _bucketName = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_BucketName");

        Connection? _connection = null;
        Input? _input = null;
        Options? _options = null;


        /// <summary>
        /// List all objects.
        /// </summary>
        [TestMethod]
        public void ListAllTest()
        {
            _connection = new Connection
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                Region = Region.EuCentral1
            };

            _input = new Input
            {
                BucketName = _bucketName
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 1000,
                Prefix = null,
                StartAfter = null,
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = ""
            };

            var result = AmazonS3.ListObjects(_connection, _input, _options, default);
            Assert.IsNotNull(result.Result.Objects);
        }

        /// <summary>
        /// With StartAfter value. Get objects created after 2022-04-22T00:16:40+02:00.
        /// </summary>
        [TestMethod]
        public void UsingStartAfterTest()
        {
            _connection = new Connection
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                Region = Region.EuCentral1
            };

            _input = new Input
            {
                BucketName = _bucketName
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 100,
                Prefix = null,
                StartAfter = "testfolder/subfolder/20220401.txt",
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = ""
            };

            var result = AmazonS3.ListObjects(_connection, _input, _options, default);
            Assert.IsNotNull(result.Result.Objects);
        }

        /// <summary>
        /// With Prefix value. Get objects from 2020/11/23/ locations.
        /// </summary>
        [TestMethod]
        public void UsingPrefixTest()
        {
            _connection = new Connection
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                Region = Region.EuCentral1
            };

            _input = new Input
            {
                BucketName = _bucketName
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 100,
                Prefix = "2020/11/23/",
                StartAfter = null,
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = ""
            };

            var result = AmazonS3.ListObjects(_connection, _input, _options, default);
            Assert.IsNotNull(result.Result.Objects);
        }

        /// <summary>
        /// Missing AwsAccessKeyId. Authentication fail.
        /// </summary>
        [TestMethod]
        public void MissingKeyTest()
        {
            _connection = new Connection
            {
                AwsAccessKeyId = null,
                AwsSecretAccessKey = _secretAccessKey,
                Region = Region.EuCentral1
            };

            _input = new Input
            {
                BucketName = _bucketName
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 100,
                Prefix = null,
                StartAfter = null,
                ThrowErrorOnFailure = true,
                ErrorMessageOnFailure = ""
            };

            var ex = Assert.ThrowsExceptionAsync<Exception>(async () => await AmazonS3.ListObjects(_connection, _input, _options, default)).Result;
            Assert.IsTrue(ex.Message.Contains("AWS credentials missing."));
        }

        /// <summary>
        /// Missing AwsAccessKeyId with ThrowErrorOnFailure = false. Should return error result instead of throwing.
        /// </summary>
        [TestMethod]
        public void MissingKeyNoThrowTest()
        {
            _connection = new Connection
            {
                AwsAccessKeyId = null,
                AwsSecretAccessKey = _secretAccessKey,
                Region = Region.EuCentral1
            };

            _input = new Input
            {
                BucketName = _bucketName
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 100,
                Prefix = null,
                StartAfter = null,
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = ""
            };

            var result = AmazonS3.ListObjects(_connection, _input, _options, default).Result;
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.ErrorMessage.Contains("AWS credentials missing."));
            Assert.IsNotNull(result.Objects);
            Assert.AreEqual(0, result.Objects.Count);
        }

        /// <summary>
        /// Missing AwsAccessKeyId with custom error message. Should return custom error message.
        /// </summary>
        [TestMethod]
        public void MissingKeyCustomErrorMessageTest()
        {
            _connection = new Connection
            {
                AwsAccessKeyId = null,
                AwsSecretAccessKey = _secretAccessKey,
                Region = Region.EuCentral1
            };

            _input = new Input
            {
                BucketName = _bucketName
            };

            _options = new Options
            {
                Delimiter = null,
                MaxKeys = 100,
                Prefix = null,
                StartAfter = null,
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = "Custom authentication error occurred"
            };

            var result = AmazonS3.ListObjects(_connection, _input, _options, default).Result;
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Custom authentication error occurred", result.ErrorMessage);
            Assert.IsNotNull(result.Objects);
            Assert.AreEqual(0, result.Objects.Count);
        }
    }
}
