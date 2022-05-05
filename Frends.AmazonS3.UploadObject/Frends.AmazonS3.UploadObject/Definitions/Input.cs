﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AmazonS3.UploadObject.Definitions
{
    /// <summary>
    /// Input parameters.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Source directory.
        /// </summary>
        /// <example>c:\temp, \\network\folder</example>
        public string FilePath { get;  set; }

        /// <summary>
        /// Windows-style filemask. Empty field = all objects (*).
        /// Only one object will be uploaded when using pre-signed URL. Consider using .zip (for example) when uploading multiple objects at the same time.
        /// </summary>
        /// <example>*.* , ?_file.*, foo_*.txt</example>
        [DefaultValue("*")]
        public string FileMask { get;  set; }

        /// <summary>
        /// AWS S3 root directory. If directory does not exist, it will be created.
        /// </summary>
        /// <example>directory/</example>
        [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.AWSCredentials)]
        public string S3Directory { get;  set; }

        /// <summary>
        /// Enable/disable AWS S3 access control list. Only for AWSCredentials-authentication method because pre-signed URL's ACL is handled in URL itself. 
        /// </summary>
        /// <example>false</example>
        [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.AWSCredentials)]
        public bool UseACL { get;  set; }

        /// <summary>
        /// Access control list.
        /// </summary>
        /// <example>Private</example>
        [UIHint(nameof(UseACL), "", true)]
        public ACLs ACL { get;  set; }
    }
}

