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
        /// Error message if the operation failed.
        /// </summary>
        public string ErrorMessage { get; private set; }

        internal Result(List<BucketObject> bucketObject)
        {
            ObjectList = bucketObject;
            Success = true;
            ErrorMessage = string.Empty;
        }

        internal Result(string errorMessage)
        {
            ObjectList = new List<BucketObject>();
            Success = false;
            ErrorMessage = errorMessage;
        }
    }
}

