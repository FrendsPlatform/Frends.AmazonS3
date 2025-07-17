using System.ComponentModel;

namespace Frends.AmazonS3.DeleteObject.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Array of objects to be deleted.
    /// </summary>
    /// <example>[ ExampleBucket, ExampleKey, 1 ], [ ExampleBucket, ExampleKey, 'empty' ]</example>
    public S3ObjectArray[] Objects { get; set; }
}
