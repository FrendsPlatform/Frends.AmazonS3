using Amazon;
using Amazon.S3;
using Frends.AmazonS3.UploadObject.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AmazonS3.UploadObject.Tests;

/// <summary>
/// Without an explicit, bounded timeout a stalled network operation (dropped packets,
/// network partition, unresponsive endpoint) never completes and never throws, so the Task
/// hangs indefinitely. These tests do not require real AWS credentials.
/// </summary>
[TestClass]
public class NetworkTimeoutTests
{
    private string dir = null!;
    private string filePath = null!;
    private TcpListener? listener;

    [TestInitialize]
    public void Initialize()
    {
        dir = Path.Combine(Environment.CurrentDirectory, "NetworkTimeoutTest");
        Directory.CreateDirectory(dir);
        filePath = Path.Combine(dir, "upload.txt");
        File.WriteAllText(filePath, "some content to upload");
    }

    [TestCleanup]
    public void Cleanup()
    {
        listener?.Stop();

        if (Directory.Exists(dir))
            Directory.Delete(dir, true);
    }

    /// <summary>
    /// Starts a TCP listener that accepts connections but never writes a response, simulating a
    /// stalled/unresponsive endpoint behind a pre-signed URL.
    /// </summary>
    private int StartUnresponsiveServer()
    {
        listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;

        _ = Task.Run(async () =>
        {
            try
            {
                using var client = await listener.AcceptTcpClientAsync();
                // Keep the connection open without ever responding, so any read/write on it stalls.
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
            catch
            {
                // Listener was stopped by test cleanup - ignore.
            }
        });

        return port;
    }

    [TestMethod]
    [Timeout(15000)]
    public async Task UploadFilePreSignedUrl_TimesOut_WhenEndpointIsUnresponsive()
    {
        var port = StartUnresponsiveServer();

        var input = new Input
        {
            SourceDirectory = dir,
            FileMask = null,
            TargetDirectory = null,
            BucketName = null,
            UploadFromCurrentDirectoryOnly = false,
            PreserveFolderStructure = false,
            DeleteSource = false,
        };
        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethod.PreSignedUrl,
            PreSignedUrl = $"http://127.0.0.1:{port}/upload.txt",
            NetworkTimeoutInSeconds = 2,
            Overwrite = false,
            ReturnListOfObjectKeys = false,
            UseMultipartUpload = false,
            GatherDebugLog = true,
            Acl = default,
            UseAcl = false,
        };
        var options = new Options
        {
            ThrowErrorIfNoMatch = false,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = "",
        };

        var uploadTask = AmazonS3.UploadObject(input, connection, options, CancellationToken.None);
        var completedTask = await Task.WhenAny(uploadTask, Task.Delay(TimeSpan.FromSeconds(10)));

        Assert.AreSame(uploadTask, completedTask,
            "Network operations must be bounded by Connection.NetworkTimeoutInSeconds instead of being able to hang indefinitely.");

        var result = await uploadTask;
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("timed out", StringComparison.OrdinalIgnoreCase),
            $"Expected a timeout-related error message, got: {result.Error.Message}");
    }

    [TestMethod]
    public void CreateS3Config_AppliesConfiguredNetworkTimeout()
    {
        var connection = new Connection
        {
            Region = Region.EuCentral1,
            NetworkTimeoutInSeconds = 42,
        };

        var method = typeof(AmazonS3).GetMethod("CreateS3Config", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method, "Could not find private method CreateS3Config via reflection.");

        var config = (AmazonS3Config)method.Invoke(null, [connection])!;

        Assert.AreEqual(TimeSpan.FromSeconds(42), config.Timeout,
            "AmazonS3Config.Timeout should be bounded by Connection.NetworkTimeoutInSeconds");
        Assert.AreEqual(RegionEndpoint.EUCentral1, config.RegionEndpoint);
    }

    [TestMethod]
    public void PreSignedUrlHttpClient_IsSharedAcrossCalls_ToAvoidSocketExhaustion()
    {
        var field = typeof(AmazonS3).GetField("s_httpClient", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(field, "Expected a single shared, static HttpClient field to avoid creating a new " +
                                "HttpClient per pre-signed URL upload, which can exhaust available sockets under repeated use.");

        var first = field.GetValue(null);
        var second = field.GetValue(null);
        Assert.IsNotNull(first);
        Assert.AreSame(first, second, "HttpClient instance should be reused across calls.");
    }
}
