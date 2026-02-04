using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Frends.AmazonS3.ListObjectVersions.Definitions;

namespace Frends.AmazonS3.ListObjectVersions.Helpers;

internal static class AmazonS3Handler
{
    internal static async Task<Result> ListObjectVersions(
        AmazonS3Client client,
        Input input,
        Options options,
        CancellationToken cancellationToken)
    {
        var result = new Result();
        var request = new ListVersionsRequest
        {
            BucketName = input.BucketName,
            MaxKeys = options.MaxKeys,
            Prefix = string.IsNullOrWhiteSpace(options.Prefix) ? null : options.Prefix,
        };

        var response = await client.ListVersionsAsync(request, cancellationToken).ConfigureAwait(false);
        result.Objects.AddRange(response.Versions.Select(entry => new BucketObjectVersions
        {
            BucketName = entry.BucketName,
            Etag = entry.ETag,
            Key = entry.Key,
            Version = entry.VersionId,
        }));
        result.ResponseTruncated = response.IsTruncated ?? false;

        return result;
    }

    internal static async Task CheckVersioningSetup(
        AmazonS3Client client,
        Input input,
        CancellationToken cancellationToken)
    {
        var response = await client
            .GetBucketVersioningAsync(
                new GetBucketVersioningRequest
                {
                    BucketName = input.BucketName,
                },
                cancellationToken)
            .ConfigureAwait(false);

        if (response.VersioningConfig.Status != VersionStatus.Enabled)
            throw new Exception("Versioning is not enabled on bucket.");
    }

    [ExcludeFromCodeCoverage(Justification = "Simple enum mapping.")]
    internal static RegionEndpoint RegionSelection(Region region)
    {
        return region switch
        {
            Region.AfSouth1 => RegionEndpoint.AFSouth1,
            Region.ApEast1 => RegionEndpoint.APEast1,
            Region.ApNortheast1 => RegionEndpoint.APNortheast1,
            Region.ApNortheast2 => RegionEndpoint.APNortheast2,
            Region.ApNortheast3 => RegionEndpoint.APNortheast3,
            Region.ApSouth1 => RegionEndpoint.APSouth1,
            Region.ApSoutheast1 => RegionEndpoint.APSoutheast1,
            Region.ApSoutheast2 => RegionEndpoint.APSoutheast2,
            Region.CaCentral1 => RegionEndpoint.CACentral1,
            Region.CnNorth1 => RegionEndpoint.CNNorth1,
            Region.CnNorthWest1 => RegionEndpoint.CNNorthWest1,
            Region.EuCentral1 => RegionEndpoint.EUCentral1,
            Region.EuNorth1 => RegionEndpoint.EUNorth1,
            Region.EuSouth1 => RegionEndpoint.EUSouth1,
            Region.EuWest1 => RegionEndpoint.EUWest1,
            Region.EuWest2 => RegionEndpoint.EUWest2,
            Region.EuWest3 => RegionEndpoint.EUWest3,
            Region.MeSouth1 => RegionEndpoint.MESouth1,
            Region.SaEast1 => RegionEndpoint.SAEast1,
            Region.UsEast1 => RegionEndpoint.USEast1,
            Region.UsEast2 => RegionEndpoint.USEast2,
            Region.UsWest1 => RegionEndpoint.USWest1,
            Region.UsWest2 => RegionEndpoint.USWest2,
            _ => RegionEndpoint.EUWest1,
        };
    }
}
