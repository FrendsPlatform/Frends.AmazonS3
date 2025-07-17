namespace Frends.AmazonS3.ListObjects.Definitions
{
    /// <summary>
    /// Input parameters required for the S3 ListObjects operation.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// The name of the S3 bucket to list objects from.
        /// </summary>
        /// <example>my-s3-bucket</example>
        public string BucketName { get; set; }
    }
}


