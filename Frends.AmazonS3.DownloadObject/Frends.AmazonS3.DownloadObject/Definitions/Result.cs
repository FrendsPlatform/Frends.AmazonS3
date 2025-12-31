using System.Collections.Generic;

namespace Frends.AmazonS3.DownloadObject.Definitions;

/// <summary>
/// Result.
/// </summary>
public class Result
{
    internal Result(bool success, List<SingleResultObject> objects, Error error = null)
    {
        Success = success;
        Objects = objects;
        Error = error;
    }

    /// <summary>
    /// Task complete without errors.
    /// </summary>
    /// <example>True</example>
    public bool Success { get; private set; }

    /// <summary>
    /// List of downloaded objects.
    /// </summary>
    /// <example>{ "File.txt", "C:\temp\File.txt", true, false, "Additional information" }</example>
    public List<SingleResultObject> Objects { get; private set; }

    /// <summary>
    /// Error information if the operation failed.
    /// </summary>
    /// <example>{ "An error occurred", { "ErrorCode": 500 } }</example>
    public Error Error { get; private set; }
}