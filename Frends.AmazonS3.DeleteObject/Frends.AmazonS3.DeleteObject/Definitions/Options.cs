﻿using System.ComponentModel;

namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Optional parameters for controlling task behavior and error handling.
/// </summary>
public class Options
{
    /// <summary>
    /// Timeout in seconds for S3 operations. If an operation takes longer than this value, it will be cancelled.
    /// </summary>
    /// <example>30</example>
    [DefaultValue(30)]
    public int Timeout { get; set; }

    /// <summary>
    /// If true, throws an exception when any delete operation fails. If false, returns a Result object with error information.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ThrowErrorOnFailure { get; set; }

    /// <summary>
    /// Custom error message to use when operations fail. If empty, default error messages will be used.
    /// </summary>
    /// <example>"Custom delete operation failed"</example>
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; }
}
