﻿using System.ComponentModel;
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

        /// <summary>
        /// Gets or sets a value indicating whether an error should stop the Task and throw an exception.
        /// If set to true, an exception will be thrown when an error occurs. If set to false, Task will try to continue and the error message will be added into Result.ErrorMessage and Result.Success will be set to false.
        /// </summary>
        /// <example>true</example>
        [DefaultValue(true)]
        public bool ThrowErrorOnFailure { get; set; }

        /// <summary>
        /// Gets or sets a custom error message that will be used when an error occurs and ThrowErrorOnFailure is true.
        /// When ThrowErrorOnFailure is false, this message will be combined with the task's own error message.
        /// </summary>
        /// <example>Custom error message for bucket creation failure.</example>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("Failed to create Amazon S3 bucket.")]
        public string ErrorMessageOnFailure { get; set; } = "Failed to create Amazon S3 bucket.";
    }
}