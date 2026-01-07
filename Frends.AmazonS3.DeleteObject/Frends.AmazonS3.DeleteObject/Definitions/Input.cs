using System.ComponentModel;

namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Array of S3 objects to be deleted. Each object contains bucket name, key, and optional version ID.
    /// </summary>
    /// <example>[ { BucketName: "ExampleBucket", Key: "ExampleKey", VersionId: "1" }, { BucketName: "ExampleBucket", Key: "ExampleKey2", VersionId: "" } ]</example>
    [DefaultValue(null)]
    public S3ObjectArray[] Objects { get; set; }

    /// <summary>
    /// Defines how to handle objects that don't exist:
    /// - None (Default): Task doesn't check if the object exists before deletion. Each delete operation will return version ID and be included in DeletedObjects unless an exception occurs.
    /// - Info: Task will check if the object exists before each delete operation. Non-existing objects will be skipped and included in DeletedObjects with null version ID.
    /// - Throw: Throw an exception if any object doesn't exist. Task will check all objects in Input.Objects before starting the delete process.
    /// </summary>
    /// <example>NotExistsHandler.None</example>
    [DefaultValue(NotExistsHandler.None)]
    public NotExistsHandler ActionOnObjectNotFound { get; set; }
}
