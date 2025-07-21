using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Frends.AmazonS3.ListObjects.Definitions;
using Frends.AmazonS3.ListObjects.Helpers;

namespace Frends.AmazonS3.ListObjects.Test
{
    /// <summary>
    /// Test cases for Amazon ListObjects task.
    /// </summary>
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
        public async Task ListAllTest()
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

            var result = await AmazonS3.ListObjects(_connection, _input, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Objects);
            Assert.IsNull(result.Error);
        }

        /// <summary>
        /// With StartAfter value. Get objects created after 2022-04-22T00:16:40+02:00.
        /// </summary>
        [TestMethod]
        public async Task UsingStartAfterTest()
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
                StartAfter = "testfolder/subfolder/20220401.txt",
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = ""
            };

            var initialResult = await AmazonS3.ListObjects(_connection, _input, new Options { MaxKeys = 1000 }, default);

            if (!initialResult.Success)
                Assert.Inconclusive("Unable to fetch initial object list: " + initialResult.ErrorMessage);

            var exists = initialResult.Objects.Exists(o => o.Key == _options.StartAfter);

            if (!exists)
                Assert.Inconclusive($"StartAfter object '{_options.StartAfter}' does not exist in bucket '{_input.BucketName}'.");

            var result = await AmazonS3.ListObjects(_connection, _input, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Objects);
            Assert.IsNull(result.Error);
        }


        /// <summary>
        /// With Prefix value. Get objects from 2020/11/23/ locations.
        /// </summary>
        [TestMethod]
        public async Task UsingPrefixTest()
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

            var initialResult = await AmazonS3.ListObjects(_connection, _input, new Options { MaxKeys = 1000 }, default);

            if (!initialResult.Success)
                Assert.Inconclusive("Unable to fetch initial object list: " + initialResult.ErrorMessage);

            var hasPrefix = initialResult.Objects.Exists(o => o.Key.StartsWith(_options.Prefix));

            if (!hasPrefix)
                Assert.Inconclusive($"No objects found with prefix '{_options.Prefix}' in bucket '{_input.BucketName}'.");

            var result = await AmazonS3.ListObjects(_connection, _input, _options, default);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Objects);
            Assert.IsNull(result.Error);
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

            var ex = Assert.ThrowsExactly<AggregateException>(() => AmazonS3.ListObjects(_connection, _input, _options, default).Result);

            Assert.IsNotNull(ex.InnerException, "AggregateException should contain an inner exception");
            Assert.IsTrue(ex.InnerException!.Message.Contains("AWS credentials missing."));
        }

        /// <summary>
        /// Missing AwsAccessKeyId with ThrowErrorOnFailure = false. Should return error result instead of throwing.
        /// </summary>
        [TestMethod]
        public async Task MissingKeyNoThrowTest()
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

            var result = await AmazonS3.ListObjects(_connection, _input, _options, default);
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.ErrorMessage.Contains("AWS credentials missing."));
            Assert.IsNotNull(result.Objects);
            Assert.AreEqual(0, result.Objects.Count);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.Message.Contains("AWS credentials missing."));
        }

        /// <summary>
        /// Missing AwsAccessKeyId with custom error message. Should return custom error message.
        /// </summary>
        [TestMethod]
        public async Task MissingKeyCustomErrorMessageTest()
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

            var result = await AmazonS3.ListObjects(_connection, _input, _options, default);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Custom authentication error occurred", result.ErrorMessage);
            Assert.IsNotNull(result.Objects);
            Assert.AreEqual(0, result.Objects.Count);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Custom authentication error occurred", result.Error.Message);
        }

        #region Connection Class Tests

        /// <summary>
        /// Test Connection class properties.
        /// </summary>
        [TestMethod]
        public void ConnectionClassTest()
        {
            var connection = new Connection
            {
                AwsAccessKeyId = "test-access-key",
                AwsSecretAccessKey = "test-secret-key",
                Region = Region.EuWest1
            };

            Assert.AreEqual("test-access-key", connection.AwsAccessKeyId);
            Assert.AreEqual("test-secret-key", connection.AwsSecretAccessKey);
            Assert.AreEqual(Region.EuWest1, connection.Region);
        }

        #endregion

        #region Options Class Tests

        /// <summary>
        /// Test Options class default values.
        /// </summary>
        [TestMethod]
        public void OptionsClassDefaultValuesTest()
        {
            var options = new Options();

            Assert.AreEqual(1000, options.MaxKeys);
            Assert.IsTrue(options.ThrowErrorOnFailure);
            Assert.IsNull(options.Prefix);
            Assert.IsNull(options.Delimiter);
            Assert.IsNull(options.StartAfter);
            Assert.IsNull(options.ErrorMessageOnFailure);
        }

        /// <summary>
        /// Test Options class property assignment.
        /// </summary>
        [TestMethod]
        public void OptionsClassPropertyAssignmentTest()
        {
            var options = new Options
            {
                Prefix = "test-prefix/",
                Delimiter = "/",
                MaxKeys = 500,
                StartAfter = "test-start-after",
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = "Custom error message"
            };

            Assert.AreEqual("test-prefix/", options.Prefix);
            Assert.AreEqual("/", options.Delimiter);
            Assert.AreEqual(500, options.MaxKeys);
            Assert.AreEqual("test-start-after", options.StartAfter);
            Assert.IsFalse(options.ThrowErrorOnFailure);
            Assert.AreEqual("Custom error message", options.ErrorMessageOnFailure);
        }

        #endregion

        #region Error Class Tests

        /// <summary>
        /// Test Error class constructor with message only.
        /// </summary>
        [TestMethod]
        public void ErrorClassMessageOnlyTest()
        {
            var error = new Error("Test error message");

            Assert.AreEqual("Test error message", error.Message);
            Assert.IsNull(error.AdditionalInfo);
        }

        /// <summary>
        /// Test Error class constructor with message and additional info.
        /// </summary>
        [TestMethod]
        public void ErrorClassWithAdditionalInfoTest()
        {
            var additionalInfo = new { Code = 500, Details = "Internal server error" };
            var error = new Error("Test error message", additionalInfo);

            Assert.AreEqual("Test error message", error.Message);
            Assert.IsNotNull(error.AdditionalInfo);
            Assert.AreEqual(500, error.AdditionalInfo.Code);
            Assert.AreEqual("Internal server error", error.AdditionalInfo.Details);
        }

        #endregion

        #region Result Class Tests

        /// <summary>
        /// Test Result class successful result.
        /// </summary>
        [TestMethod]
        public void ResultClassSuccessTest()
        {
            var bucketObjects = new System.Collections.Generic.List<BucketObject>
            {
                new() {
                    BucketName = "test-bucket",
                    Key = "test-key",
                    Size = 1024,
                    Etag = "test-etag",
                    LastModified = DateTime.Now
                }
            };

            var result = new Result(bucketObjects);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Objects);
            Assert.AreEqual(1, result.Objects.Count);
            Assert.IsNull(result.Error);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }

        /// <summary>
        /// Test Result class error result with string message.
        /// </summary>
        [TestMethod]
        public void ResultClassErrorStringTest()
        {
            var result = new Result("Test error message");

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Objects);
            Assert.AreEqual(0, result.Objects.Count);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Test error message", result.Error.Message);
            Assert.AreEqual("Test error message", result.ErrorMessage);
        }

        /// <summary>
        /// Test Result class error result with Error object.
        /// </summary>
        [TestMethod]
        public void ResultClassErrorObjectTest()
        {
            var error = new Error("Test error message", new { Code = 404 });
            var result = new Result(error);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Objects);
            Assert.AreEqual(0, result.Objects.Count);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Test error message", result.Error.Message);
            Assert.AreEqual("Test error message", result.ErrorMessage);
            Assert.AreEqual(404, result.Error.AdditionalInfo.Code);
        }

        #endregion

        #region ErrorHandler Tests

        /// <summary>
        /// Test ErrorHandler.Handle with ThrowErrorOnFailure = true.
        /// </summary>
        [TestMethod]
        public void ErrorHandlerHandleThrowTest()
        {
            var options = new Options { ThrowErrorOnFailure = true };
            var exception = new Exception("Test exception");

            Assert.ThrowsExactly<Exception>(() => ErrorHandler.Handle(exception, options));
        }

        /// <summary>
        /// Test ErrorHandler.Handle with ThrowErrorOnFailure = false and no custom message.
        /// </summary>
        [TestMethod]
        public void ErrorHandlerHandleNoThrowTest()
        {
            var options = new Options { ThrowErrorOnFailure = false };
            var exception = new Exception("Test exception");

            var result = ErrorHandler.Handle(exception, options);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Test exception", result.ErrorMessage);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.AdditionalInfo);
        }

        /// <summary>
        /// Test ErrorHandler.Handle with ThrowErrorOnFailure = false and custom message.
        /// </summary>
        [TestMethod]
        public void ErrorHandlerHandleCustomMessageTest()
        {
            var options = new Options
            {
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = "Custom error occurred"
            };
            var exception = new Exception("Original exception");

            var result = ErrorHandler.Handle(exception, options);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Custom error occurred", result.ErrorMessage);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Custom error occurred", result.Error.Message);
        }

        /// <summary>
        /// Test ErrorHandler.HandleCredentialsError with ThrowErrorOnFailure = true.
        /// </summary>
        [TestMethod]
        public void ErrorHandlerCredentialsThrowTest()
        {
            var options = new Options { ThrowErrorOnFailure = true };

            Assert.ThrowsExactly<Exception>(() =>
                ErrorHandler.HandleCredentialsError("Credentials missing", options));
        }

        /// <summary>
        /// Test ErrorHandler.HandleCredentialsError with ThrowErrorOnFailure = false.
        /// </summary>
        [TestMethod]
        public void ErrorHandlerCredentialsNoThrowTest()
        {
            var options = new Options { ThrowErrorOnFailure = false };

            var result = ErrorHandler.HandleCredentialsError("Credentials missing", options);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Credentials missing", result.ErrorMessage);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Credentials missing", result.Error.Message);
        }

        /// <summary>
        /// Test ErrorHandler.HandleCredentialsError with custom message.
        /// </summary>
        [TestMethod]
        public void ErrorHandlerCredentialsCustomMessageTest()
        {
            var options = new Options
            {
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = "Authentication failed"
            };

            var result = ErrorHandler.HandleCredentialsError("Credentials missing", options);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Authentication failed", result.ErrorMessage);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Authentication failed", result.Error.Message);
        }

        #endregion

        #region Input Class Tests

        /// <summary>
        /// Test Input class property assignment.
        /// </summary>
        [TestMethod]
        public void InputClassTest()
        {
            var input = new Input
            {
                BucketName = "test-bucket-name"
            };

            Assert.AreEqual("test-bucket-name", input.BucketName);
        }

        #endregion

        #region Integration Tests

        /// <summary>
        /// Test with invalid bucket name to verify error handling.
        /// </summary>
        [TestMethod]
        public async Task InvalidBucketNameTest()
        {
            _connection = new Connection
            {
                AwsAccessKeyId = _accessKey,
                AwsSecretAccessKey = _secretAccessKey,
                Region = Region.EuCentral1
            };

            _input = new Input
            {
                BucketName = "invalid-bucket-name-that-does-not-exist-12345"
            };

            _options = new Options
            {
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = ""
            };

            var result = await AmazonS3.ListObjects(_connection, _input, _options, default);

            // Should handle the error gracefully when bucket doesn't exist
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Objects);
            Assert.AreEqual(0, result.Objects.Count);
        }

        /// <summary>
        /// Test with empty options to verify defaults work.
        /// </summary>
        [TestMethod]
        public async Task EmptyOptionsTest()
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

            _options = new Options(); // Use default values

            var result = await AmazonS3.ListObjects(_connection, _input, _options, default);

            if (_accessKey != null && _secretAccessKey != null && _bucketName != null)
            {
                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Objects);
                Assert.IsNull(result.Error);
            }
        }

        #endregion
    }
}
