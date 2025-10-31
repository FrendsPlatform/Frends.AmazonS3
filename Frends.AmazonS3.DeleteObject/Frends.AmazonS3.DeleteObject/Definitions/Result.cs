using System.Collections.Generic;

namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Result object containing the outcome of the delete operation(s).
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether all delete operations completed successfully. False if any operations failed or objects were not found (depending on ActionOnObjectNotFound setting).
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// List of objects that were processed during the delete operation. Includes both successfully deleted objects and skipped objects (when ActionOnObjectNotFound is set to Info).
    /// </summary>
    /// <example>[ { BucketName: "my-bucket", Key: "Key1.txt", VersionId: "etZwWf8lJPf_5MuzOyFzepWqA3eS3EIN" }, { BucketName: "my-bucket", Key: "Key2.txt", VersionId: "atZwWf8lJPf_5MuzOyFzepWqA3eS3EIN" } ]</example>
    public List<SingleResultObject> DeletedObjects { get; private set; }

    /// <summary>
    /// Error information containing details about failed operations and objects that encountered errors. Null if all operations succeeded.
    /// </summary>
    public Error Error { get; private set; }

    /// <summary>
    /// Initializes a new result indicating whether the delete operation succeeded, along with deleted items and any error info.
    /// </summary>
    public Result(bool success, List<SingleResultObject> deletedObjects, Error error = null)
    {
        Success = success;
        DeletedObjects = deletedObjects;
        Error = error;
    }
}
