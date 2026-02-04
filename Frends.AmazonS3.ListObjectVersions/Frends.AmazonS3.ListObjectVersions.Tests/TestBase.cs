using System;
using dotenv.net;
using Frends.AmazonS3.ListObjectVersions.Definitions;

namespace Frends.AmazonS3.ListObjectVersions.Tests;

public abstract class TestBase
{
    protected TestBase()
    {
        DotEnv.Load();
        AccessKey = Environment.GetEnvironmentVariable("ACCESS_KEY");
        SecretAccessKey = Environment.GetEnvironmentVariable("SECRET_ACCESS_KEY");
    }

    protected string BucketName { get; } = $"test-bucket-list-object-versions-{Guid.NewGuid().ToString("N")[..8]}";

    protected string AccessKey { get; }

    protected string SecretAccessKey { get; }

    protected static Options DefaultOptions() => new()
    {
        ThrowErrorOnFailure = true,
        ErrorMessageOnFailure = string.Empty,
    };

    protected Input DefaultInput() => new()
    {
        BucketName = BucketName,
    };

    protected Connection DefaultConnection() => new()
    {
        AwsAccessKeyId = AccessKey,
        AwsSecretAccessKey = SecretAccessKey,
        Region = Region.EuCentral1,
    };
}
