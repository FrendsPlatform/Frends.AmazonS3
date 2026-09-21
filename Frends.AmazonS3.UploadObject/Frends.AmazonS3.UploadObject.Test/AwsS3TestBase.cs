using dotenv.net;
using System;

namespace Frends.AmazonS3.UploadObject.Tests;

public abstract class AwsS3TestBase
{
    protected const string BucketPrefix = "frends-upload-object-tests-";

    protected static string BucketName { get; } = $"{BucketPrefix}{Guid.NewGuid():N}";

    protected static string? AccessKey { get; }

    protected static string? SecretAccessKey { get; }

    static AwsS3TestBase()
    {
        DotEnv.Load();

        AccessKey = Environment.GetEnvironmentVariable("ACCESS_KEY");
        SecretAccessKey = Environment.GetEnvironmentVariable("SECRET_ACCESS_KEY");
    }
}
