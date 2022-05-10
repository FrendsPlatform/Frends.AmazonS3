﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.DownloadObject
{
    /// <summary>
    /// Input parameters.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Filter options by authentication method.
        /// </summary>
        /// <example>AWS Credentials</example>
        [DefaultValue(OptionsFor.AWSCredentials)]
        public OptionsFor OptionsFor { get; set; }

        /// <summary>
        /// Downloads all objects with this prefix.
        /// </summary>
        /// <example>directory/</example>
        [UIHint(nameof(OptionsFor), "", OptionsFor.AWSCredentials)]
        [DisplayFormat(DataFormatString = "Text")]
        public string S3Directory { get; set; }

        /// <summary>
        /// String pattern to search files.
        /// </summary>
        /// <example>*.*, *file?.txt</example>
        [UIHint(nameof(OptionsFor), "", OptionsFor.AWSCredentials)]
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("*")]
        public string SearchPattern { get; set; }

        /// <summary>
        /// Directory where to create folders and files.
        /// </summary>
        /// <example>c:\temp, \\network\folder</example>
        [DisplayFormat(DataFormatString = "Text")]
        public string DestinationPath { get; set; }

        /// <summary>
        /// Set filename when using pre-signed URL.
        /// </summary>
        /// <example>File.txt</example>
        [UIHint(nameof(OptionsFor), "", OptionsFor.PreSignedURL)]
        [DisplayFormat(DataFormatString = "Text")]
        public string FileName { get; set; }
    }
}

