namespace Frends.AmazonS3.DownloadObject.Helpers;

using System.Text.RegularExpressions;

/// <summary>
/// Class to store custom regexes.
/// </summary>
internal static partial class RegexCatalog
{
    [GeneratedRegex(@"[^\/]+(?=\?)")]
    internal static partial Regex ObjectNameFromUri();
}