﻿using System.ComponentModel;
namespace Frends.AmazonS3.ListObjects.Definitions
{
    /// <summary>
    /// Input values.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// The name of the bucket containing this object.
        /// </summary>
        /// <example>ObjectName</example>
        public string BucketName { get; set; }

        /// <summary>
        /// AWS Access Key ID.
        /// </summary>
        /// <example>AKIAQWERTY7NJ5Q7NZ6Q</example>
        [PasswordPropertyText]
        public string AwsAccessKeyId { get; set; }

        /// <summary>
        /// AWS Secret Access Key.
        /// </summary>
        /// <example>TVh5hgd3uGY/2CqH+Kkrrg3dadbXLsYe0jC3h+WD</example>
        [PasswordPropertyText]
        public string AwsSecretAccessKey { get; set; }

        /// <summary>
        /// AWS Region selection.
        /// </summary>
        /// <example>EuCentral1</example>
        public Region Region { get; set; }
    }
}


