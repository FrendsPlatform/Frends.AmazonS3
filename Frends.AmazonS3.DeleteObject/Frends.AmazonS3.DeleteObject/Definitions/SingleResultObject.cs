namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Single delete operation result.
/// </summary>
public class SingleResultObject
{
    /// <summary>
    /// The bucket name containing the deleted object.
    /// </summary>
    /// <example>my-bucket</example>
    public string BucketName { get; set; }

    /// <summary>
    /// The key identifying the deleted object.
    /// </summary>
    /// <example>ExampleKey</example>
    public string Key { get; set; }

    /// <summary>
    /// Returns the version ID of the delete marker created as a result of the DELETE operation.
    /// </summary>
    /// <example>q97fnr1zy_gsDcPAMbbwoW2eY0wgoFPt</example>
    public string VersionId { get; set; }
}
