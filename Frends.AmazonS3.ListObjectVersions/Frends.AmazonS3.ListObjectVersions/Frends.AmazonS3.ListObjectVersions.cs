using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Frends.AmazonS3.ListObjectVersions.Definitions;
using Frends.AmazonS3.ListObjectVersions.Helpers;

namespace Frends.AmazonS3.ListObjectVersions;

/// <summary>
/// Task Class for AmazonS3 operations.
/// </summary>
public static class AmazonS3
{
    /// <summary>
    /// Task to get metadata about all versions of the objects in a bucket
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-AmazonS3-ListObjectVersions)
    /// </summary>
    /// <param name="input">Essential parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>object { bool Success, BucketObjectVersions[] Objects, object Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> ListObjectVersions(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connection.AwsSecretAccessKey) ||
                string.IsNullOrWhiteSpace(connection.AwsAccessKeyId))
            {
                throw new ArgumentException("AWS credentials missing.", nameof(connection));
            }

            var region = AmazonS3Handler.RegionSelection(connection.Region);
            using var client = new AmazonS3Client(connection.AwsAccessKeyId, connection.AwsSecretAccessKey, region);
            await AmazonS3Handler.CheckVersioningSetup(client, input, cancellationToken).ConfigureAwait(false);
            var result = await AmazonS3Handler.ListObjectVersions(client, input, options, cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            return ErrorHandler.Handle(ex, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
        }
    }
}
