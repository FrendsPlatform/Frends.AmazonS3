using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.ListObjects.Definitions
{
    /// <summary>
    /// Configuration options for customizing the S3 ListObjects operation behavior.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Limits the response to keys that begin with the specified prefix.
        /// </summary>
        /// <example>bucket/object1/object2</example>
        public string Prefix { get; set; }

        /// <summary>
        /// A delimiter character used to group keys hierarchically.
        /// See: http://docs.aws.amazon.com/AmazonS3/latest/dev/ListingKeysHierarchy.html
        /// </summary>
        /// <example>/</example>
        public string Delimiter { get; set; }

        /// <summary>
        /// Sets the maximum number of keys returned in the response.
        /// The response might contain fewer keys but will never contain more than this value.
        /// </summary>
        /// <example>1000</example>
        [DefaultValue(1000)]
        public int MaxKeys { get; set; }

        /// <summary>
        /// StartAfter specifies the key to start listing from (exclusive).
        /// For example, in a bucket with objects { 20220401, 20220402, 20220403 },
        /// StartAfter=20220401 returns objects 20220402 and 20220403.
        /// </summary>
        /// <example>20220401</example>
        public string StartAfter { get; set; }

        /// <summary>
        /// Determines whether to throw an exception on failure.
        /// If set to false, returns a Result object with error information instead.
        /// </summary>
        /// <example>true</example>
        [DefaultValue(true)]
        public bool ThrowErrorOnFailure { get; set; }

        /// <summary>
        /// Custom error message to return when ThrowErrorOnFailure is false.
        /// If not specified, the original error message will be used.
        /// </summary>
        /// <example>Custom error occurred</example>
        public string ErrorMessageOnFailure { get; set; }
    }
}


