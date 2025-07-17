using System.Collections.Generic;

namespace Frends.AmazonS3.ListObjects.Definitions
{
    /// <summary>
    /// Result.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// List of objects.
        /// </summary>
        public List<BucketObject> ObjectList { get; private set; }

        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Error information if the operation failed.
        /// </summary>
        public Error Error { get; private set; }

        /// <summary>
        /// Error message if the operation failed (for backward compatibility).
        /// </summary>
        public string ErrorMessage => Error?.Message ?? string.Empty;

        internal Result(List<BucketObject> bucketObject)
        {
            ObjectList = bucketObject;
            Success = true;
            Error = null;
        }

        internal Result(string errorMessage)
        {
            ObjectList = new List<BucketObject>();
            Success = false;
            Error = new Error(errorMessage);
        }

        internal Result(Error error)
        {
            ObjectList = new List<BucketObject>();
            Success = false;
            Error = error;
        }
    }
}

