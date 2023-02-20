﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Frends.AmazonS3.DeleteObject.Definitions;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;

namespace Frends.AmazonS3.DeleteObject;

/// <summary>
/// Amazon S3 Task.
/// </summary>
public class AmazonS3
{

    /// For mem cleanup.
    static AmazonS3()
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var currentContext = AssemblyLoadContext.GetLoadContext(currentAssembly);
        if (currentContext != null)
            currentContext.Unloading += OnPluginUnloadingRequested;
    }

    /// <summary>
    /// Delete objects from an Amazon S3 bucket.
    /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.AmazonS3.DeleteObject)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Optional parameters.</param>
    /// <param name="cancellationToken">Token generated by frends to stop this Task.</param>
    /// <returns>Object { bool Success, List { bool Success, string Key, string VersionId, string Error } }</returns>
    public static async Task<Result> DeleteObject([PropertyTab] Input input, Options options, CancellationToken cancellationToken)
    {
        var result = new List<SingleResultObject>();

        if (input.Objects is null || input.Objects.Length < 0)
            throw new Exception("DeleteObject error: Input.Objects cannot be empty.");

        try
        {
            using AmazonS3Client client = new(input.AwsAccessKeyId, input.AwsSecretAccessKey, RegionSelection(input.Region));

            // Do existing check here to skip case where some of the objects have been deleted before exception occurs.
            if (options.NotExistsHandler is NotExistsHandler.Throw)
            {
                foreach (var obj in input.Objects)
                    if (!await FileExistsInS3(client, obj.BucketName, obj.Key))
                        throw new Exception($"DeleteObject Exception: Object {obj.Key} doesn't exist in {obj.BucketName}. Delete operation(s) have been skipped.");
            }

            foreach (var obj in input.Objects)
            {
                var versionId = string.IsNullOrWhiteSpace(obj.VersionId) ? obj.VersionId : null;

                switch (options.NotExistsHandler)
                {
                    case NotExistsHandler.None:
                    case NotExistsHandler.Throw:
                    default:
                        var deleted = await DeleteS3Object(client, obj.BucketName, obj.Key, versionId, cancellationToken);
                        result.Add(new SingleResultObject() { Success = true, Key = obj.Key, VersionId = deleted.VersionId, Error = null });
                        break;
                    case NotExistsHandler.Info:
                        if (await FileExistsInS3(client, obj.BucketName, obj.Key))
                        {
                            var deleted2 = await DeleteS3Object(client, obj.BucketName, obj.Key, string.IsNullOrWhiteSpace(obj.VersionId) ? obj.VersionId : null, cancellationToken);
                            result.Add(new SingleResultObject() { Success = true, Key = obj.Key, VersionId = deleted2.VersionId, Error = null });
                        }
                        else
                            result.Add(new SingleResultObject() { Success = false, Key = obj.Key, VersionId = null, Error = $"Object {obj.Key} doesn't exist in {obj.BucketName}." });
                        break;
                }
            }

            return new Result(true, result);
        }
        catch (AmazonS3Exception aEx)
        {
            throw new AmazonS3Exception($"DeleteObject AmazonS3Exception: ", aEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"DeleteObject Exception: ", ex);
        }
    }

    private static async Task<DeleteObjectResponse> DeleteS3Object(AmazonS3Client client, string bucketName, string key, string versionId, CancellationToken cancellationToken)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            VersionId = string.IsNullOrWhiteSpace(versionId) ? versionId : null,
        };
        return await client.DeleteObjectAsync(deleteObjectRequest, cancellationToken);
    }


    private static async Task<bool> FileExistsInS3(AmazonS3Client client, string bucketName, string key)
    {
        var request = new ListObjectsRequest
        {
            BucketName = bucketName,
            Prefix = key,
        };
        ListObjectsResponse response = await client.ListObjectsAsync(request);
        return (response != null && response.S3Objects != null && response.S3Objects.Count > 0);
    }

    private static RegionEndpoint RegionSelection(Region region)
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

    private static void OnPluginUnloadingRequested(AssemblyLoadContext obj)
    {
        obj.Unloading -= OnPluginUnloadingRequested;
    }
}