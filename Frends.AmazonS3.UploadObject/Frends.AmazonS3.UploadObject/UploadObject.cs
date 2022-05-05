﻿using System;
using System.ComponentModel;
using System.Threading;
using Frends.AmazonS3.UploadObject.Definitions;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;

namespace Frends.AmazonS3.UploadObject
{
    /// <summary>
    /// Amazon S3 task.
    /// </summary>
    public class AmazonS3
    {
        /// <summary>
        /// Upload objects to AWS S3 Bucket.
        /// </summary>
        /// <param name="connection">Connection parameters</param>
        /// <param name="input">Input parameters</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Token to stop task. This is generated by Frends.</param>
        /// <returns>List { string Results }</returns>
        public static async Task<Result> UploadObject([PropertyTab] Connection connection, [PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(input.FilePath)) throw new Exception($"Source path not found. {input.FilePath}");

            switch (connection.AuthenticationMethod)
            {
                case AuthenticationMethod.AWSCredentials:
                    if (string.IsNullOrWhiteSpace(connection.AwsAccessKeyId) || string.IsNullOrWhiteSpace(connection.AwsSecretAccessKey) || string.IsNullOrWhiteSpace(connection.BucketName) || string.IsNullOrWhiteSpace(connection.BucketName))
                        throw new Exception("AWS Access Key Id and Secret Access Key required.");
                    break;

                case AuthenticationMethod.PreSignedURL:
                    if (string.IsNullOrWhiteSpace(connection.PreSignedURL))
                        throw new Exception("AWS pre-signed URL required.");
                    break;
            }

            var localRoot = new DirectoryInfo(input.FilePath);

            // If filemask is not set, get all files.
            var filesToCopy = localRoot.GetFiles(
                input.FileMask ?? "*",
                options.UploadFromCurrentDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories
            );

            if (options.ThrowErrorIfNoMatch && filesToCopy.Length < 1) throw new Exception($"No files match the filemask within supplied path. {nameof(input.FileMask)}");

            var results = await ExecuteUpload(filesToCopy, input, connection, options, cancellationToken);

            return new Result(results);
        }

        private static async Task<List<UploadResult>> ExecuteUpload(IEnumerable<FileInfo> filesToCopy, Input input, Connection connection, Options options, CancellationToken cancellationToken)
        {
            var result = new List<UploadResult>();

            foreach (var file in filesToCopy)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (file.FullName.Split(Path.DirectorySeparatorChar).Length > input.FilePath.Split(Path.DirectorySeparatorChar).Length && options.PreserveFolderStructure)
                {
                    var subfolders = file.FullName.Replace(file.Name, "").Replace(input.FilePath.Replace(file.Name, ""), "").Replace(Path.DirectorySeparatorChar, '/');

                    if (subfolders.StartsWith("/"))
                        subfolders = subfolders.Remove(0, 1);

                    var fullPath = input.S3Directory + subfolders + file.Name;

                    switch (connection.AuthenticationMethod)
                    {
                        case AuthenticationMethod.PreSignedURL:
                            await UploadFilePreSignedUrl(connection, file.FullName, cancellationToken);
                            break;
                        case AuthenticationMethod.AWSCredentials:
                            await UploadFileToS3(file, options, connection, fullPath, input, cancellationToken);
                            break;
                    }
                    result.Add(new UploadResult { UploadedObject = options.ReturnListOfObjectKeys ? fullPath : file.FullName });
                }
                else
                {
                    switch (connection.AuthenticationMethod)
                    {
                        case AuthenticationMethod.PreSignedURL:
                            await UploadFilePreSignedUrl(connection, file.FullName, cancellationToken);
                            break;
                        case AuthenticationMethod.AWSCredentials:
                            await UploadFileToS3(file, options, connection, input.S3Directory + file.Name, input, cancellationToken);
                            break;
                    }
                    result.Add(new UploadResult { UploadedObject = options.ReturnListOfObjectKeys ? input.S3Directory + file.Name : file.FullName });

                }

                if (options.DeleteSource) DeleteSourceFile(file.FullName);

                // Each file require their own presigned URL so no point to loop more than first file.
                if (connection.AuthenticationMethod == AuthenticationMethod.PreSignedURL) break;
            }
            return result;
        }

        //Can't check if file already exists without S3Client
        private static async Task<string> UploadFilePreSignedUrl(Connection connection, string path, CancellationToken cancellationToken)
        {
            try
            {
                await using var fileStream = File.OpenRead(path);
                var fileStreamResponse = await new HttpClient().PutAsync(new Uri(connection.PreSignedURL), new StreamContent(fileStream), cancellationToken);
                var response = fileStreamResponse.EnsureSuccessStatusCode();
                fileStream.Close();
                return response.IsSuccessStatusCode ? $"Upload complete. {response.Content.ToString}" : $"Upload failed. {response.Content.ToString}";
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
        }

        private static async Task<PutObjectResponse> UploadFileToS3(FileInfo file, Options options, Connection connection, string path, Input input, CancellationToken cancellationToken)
        {
            var region = RegionSelection(connection.Region);
            var client = new AmazonS3Client(connection.AwsAccessKeyId, connection.AwsSecretAccessKey, region);

            if (!options.Overwrite)
            {
                try
                {
                    var request = new GetObjectRequest
                    {
                        BucketName = connection.BucketName,
                        Key = path
                    };
                    await client.GetObjectAsync(request, cancellationToken);
                    throw new ArgumentException($"File {file.Name} already exists in S3 at {path}. Set Overwrite-option to true to overwrite the existing file.");
                }
                catch (AmazonS3Exception) { }
            }

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = connection.BucketName,
                Key = path,
                FilePath = file.FullName,
                CannedACL = (input.UseACL) ? GetS3CannedACL(input.ACL) : S3CannedACL.NoACL
            };
            var response = await client.PutObjectAsync(putObjectRequest, cancellationToken);

            return response;

        }

        private static void DeleteSourceFile(string filePath)
        {
            try
            {
                var file = new FileInfo(filePath);
                while (IsFileLocked(file)) Thread.Sleep(1000);
                File.Delete(filePath);
            }
            catch (Exception ex) { throw new Exception($"Delete failed. {ex}"); }
        }

        private static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                // The file is unavailable because it is:
                // 1. Still being written to.
                // 2. Being processed by another thread.
                // 3. Does not exist (has already been processed).
                return true;
            }
            finally { stream?.Close(); }

            // File is not locked.
            return false;
        }

        private static S3CannedACL GetS3CannedACL(ACLs acl)
        {
            return acl switch
            {
                ACLs.Private => S3CannedACL.Private,
                ACLs.PublicRead => S3CannedACL.PublicRead,
                ACLs.PublicReadWrite => S3CannedACL.PublicReadWrite,
                ACLs.AuthenticatedRead => S3CannedACL.AuthenticatedRead,
                ACLs.BucketOwnerRead => S3CannedACL.BucketOwnerRead,
                ACLs.BucketOwnerFullControl => S3CannedACL.BucketOwnerFullControl,
                ACLs.LogDeliveryWrite => S3CannedACL.LogDeliveryWrite,
                _ => S3CannedACL.NoACL,
            };
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
    }
}
