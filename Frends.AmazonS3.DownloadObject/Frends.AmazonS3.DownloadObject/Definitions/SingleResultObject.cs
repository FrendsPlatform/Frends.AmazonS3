namespace Frends.AmazonS3.DownloadObject.Definitions;

/// <summary>
/// Single download result.
/// </summary>
public class SingleResultObject
{
    /// <summary>
    /// Single download result.
    /// </summary>
    /// <example>Download complete: {fullPath}.</example>
    public string ObjectData { get; private set; }

    /// <summary>
    /// Object's name.
    /// </summary>
    /// <example>Filename.txt</example>
    public string ObjectName { get; private set; }

    /// <summary>
    /// Full path.
    /// </summary>
    /// <example>c:\temp\Filename.txt, \\network\folder\Filename.txt</example>
    public string FullPath { get; private set; }

    internal SingleResultObject(string objectData, string objectName, string fullPath)
    {
        ObjectData = objectData;
        ObjectName = objectName;
        FullPath = fullPath;
    }
}
