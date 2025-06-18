using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.CreateBucket.Definitions
{
    /// <summary>
    /// Option parameters.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Specifies whether you want S3 Object Lock to be enabled for the new bucket.
        /// </summary>
        /// <example>false</example>
        [DefaultValue(false)]
        public bool ObjectLockEnabled { get; set; }
    }
}