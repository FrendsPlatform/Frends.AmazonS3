using System.Collections.Generic;

namespace Frends.AmazonS3.ListObjects.Definitions
{
    /// <summary>
    /// Represents the result of an S3 ListObjects operation, containing either success data or error information.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// List of objects retrieved from the S3 bucket. Empty list if operation failed.
        /// </summary>
        public List<BucketObject> Objects { get; private set; }

        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Error information if the operation failed.
        /// </summary>
        public Error Error { get; private set; }

        /// <summary>
        /// Error message if the operation failed. Returns empty string if operation succeeded.
        /// Provided for backward compatibility - use Error property for detailed error information.
        /// </summary>
        public string ErrorMessage => Error?.Message ?? string.Empty;

        /// <summary>
        /// Initializes a new successful Result with the specified list of bucket objects.
        /// </summary>
        /// <param name="bucketObject">The list of bucket objects retrieved from S3</param>
        public Result(List<BucketObject> bucketObject)
        {
            Objects = bucketObject;
            Success = true;
            Error = null;
        }

        /// <summary>
        /// Initializes a new failed Result with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message describing what went wrong</param>
        public Result(string errorMessage)
        {
            Objects = new List<BucketObject>();
            Success = false;
            Error = new Error(errorMessage);
        }

        /// <summary>
        /// Initializes a new failed Result with the specified Error object.
        /// </summary>
        /// <param name="error">The Error object containing detailed error information</param>
        public Result(Error error)
        {
            Objects = new List<BucketObject>();
            Success = false;
            Error = error;
        }
    }
}

