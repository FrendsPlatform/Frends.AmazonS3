using System.Collections.Generic;

namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Result.
/// </summary>
public class Result
{
    /// <summary>
    /// Task complete.
    /// </summary>
    /// <example>True</example>
    public bool Success { get; private set; }

    /// <summary>
    /// List of successfully deleted objects.
    /// </summary>
    /// <example>{ "Key1.txt", "etZwWf8lJPf_5MuzOyFzepWqA3eS3EIN" }, { "Key2.txt", "atZwWf8lJPf_5MuzOyFzepWqA3eS3EIN" }</example>
    public List<SingleResultObject> DeletedObjects { get; private set; }

    /// <summary>
    /// Error information including objects that encountered errors.
    /// </summary>
    public Error Error { get; private set; }

    internal Result(bool success, List<SingleResultObject> deletedObjects, Error error = null)
    {
        Success = success;
        DeletedObjects = deletedObjects;
        Error = error;
    }
}
