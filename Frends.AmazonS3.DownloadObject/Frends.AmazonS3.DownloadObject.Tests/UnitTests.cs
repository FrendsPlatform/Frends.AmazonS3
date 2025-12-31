using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Frends.AmazonS3.DownloadObject.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.AmazonS3.DownloadObject.Tests;

using System.Threading;

[TestClass]
public class UnitTests
{
    private readonly string accessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_AccessKey");
    private readonly string secretAccessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_SecretAccessKey");
    private readonly string bucketName = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_BucketName");
    private readonly string dir = Path.Combine(Environment.CurrentDirectory); // .\Frends.AmazonS3.DownloadObject\Frends.AmazonS3.DownloadObject.Test\bin\Debug\net6.0\

    private Connection defaultConnection = new();
    private Input defaultInput = new();
    private Options defaultOptions = new();

    [TestInitialize]
    public async Task Initialize()
    {
        defaultConnection = new Connection()
        {
            AuthenticationMethod = AuthenticationMethods.AwsCredentials,
            AwsAccessKeyId = accessKey,
            AwsSecretAccessKey = secretAccessKey,
            Region = Region.EuCentral1,
            PreSignedUrl = null,
        };

        defaultOptions = new Options()
        {
            DeleteSourceObject = true,
            ThrowErrorIfNoMatch = true,
            ActionOnExistingFile = DestinationFileExistsActions.Overwrite,
            FileLockedRetries = 0,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = string.Empty,
        };

        defaultInput = new Input()
        {
            BucketName = bucketName,
            SourceDirectory = "DownloadTest/",
            SearchPattern = "*",
            DownloadFromCurrentDirectoryOnly = true,
            TargetDirectory = @$"{dir}\Download",
        };

        await CreateTestFiles();
    }

    [TestCleanup]
    public void CleanUp()
    {
        Directory.Delete($@"{dir}\Download", true);
        Directory.Delete($@"{dir}\DownloadTestFiles", true);
    }

    [TestMethod]
    public async Task PreSignedURL_DownloadFile_Test()
    {
        var setS3Key = $"DownloadTest/Testfile.txt";

        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = CreatePreSignedUrl(setS3Key),
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
        };

        var result = await AmazonS3.DownloadObject(defaultInput, connection, defaultOptions, CancellationToken.None);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Testfile.txt"));
    }

    [TestMethod]
    public async Task PreSignedURL_NoDestinationFilename_Test()
    {
        var setS3Key = $"DownloadTest/Testfile.txt";

        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = CreatePreSignedUrl(setS3Key),
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
        };

        var result = await AmazonS3.DownloadObject(defaultInput, connection, defaultOptions, CancellationToken.None);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Testfile.txt"));
    }

    [TestMethod]
    public async Task PreSignedURL_Overwrite_Exists_Test()
    {
        var setS3Key = $"DownloadTest/Overwrite.txt";

        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = CreatePreSignedUrl(setS3Key),
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
        };

        var result = await AmazonS3.DownloadObject(defaultInput, connection, defaultOptions, CancellationToken.None);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(result.Objects.Any(x => x.Overwritten.Equals(true)));
        Assert.IsTrue(CompareFiles());
    }

    [TestMethod]
    public async Task PreSignedURL_Info_Exists_Test()
    {
        var setS3Key = $"DownloadTest/Overwrite.txt";

        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = CreatePreSignedUrl(setS3Key),
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
        };

        var options = new Options
        {
            ActionOnExistingFile = DestinationFileExistsActions.Info,
            DeleteSourceObject = defaultOptions.DeleteSourceObject,
            ThrowErrorIfNoMatch = defaultOptions.ThrowErrorIfNoMatch,
            FileLockedRetries = defaultOptions.FileLockedRetries,
            ThrowErrorOnFailure = defaultOptions.ThrowErrorOnFailure,
            ErrorMessageOnFailure = defaultOptions.ErrorMessageOnFailure,
        };

        var result = await AmazonS3.DownloadObject(defaultInput, connection, options, CancellationToken.None);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(result.Objects.Any(x => x.Info.Contains("Object skipped because file already exists in destination")));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Overwrite.txt"));
        Assert.IsFalse(CompareFiles());
    }

    [TestMethod]
    public async Task PreSignedURL_Error_Exists_Test()
    {
        var setS3Key = $"DownloadTest/Testfile.txt";
        await File.WriteAllTextAsync(@$"{dir}\Download\Testfile.txt", "I exist");

        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = CreatePreSignedUrl(setS3Key),
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
        };

        var options = new Options
        {
            ActionOnExistingFile = DestinationFileExistsActions.Error,
            DeleteSourceObject = defaultOptions.DeleteSourceObject,
            ThrowErrorIfNoMatch = defaultOptions.ThrowErrorIfNoMatch,
            FileLockedRetries = defaultOptions.FileLockedRetries,
            ThrowErrorOnFailure = defaultOptions.ThrowErrorOnFailure,
            ErrorMessageOnFailure = defaultOptions.ErrorMessageOnFailure,
        };

        var result = await AmazonS3.DownloadObject(defaultInput, connection, options, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("already exists"));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Testfile.txt"));
    }

    [TestMethod]
    public async Task PreSignedURL_MissingURL_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = string.Empty,
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
        };

        var result = await AmazonS3.DownloadObject(defaultInput, connection, defaultOptions, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("AWS pre-signed URL required."));
    }

    [TestMethod]
    public async Task PreSignedURL_MissingDestinationDirectory_Test()
    {
        var setS3Key = $"DownloadTest/Testfile.txt";

        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = CreatePreSignedUrl(setS3Key),
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
        };

        var input = new Input
        {
            BucketName = defaultInput.BucketName,
            SourceDirectory = defaultInput.SourceDirectory,
            SearchPattern = defaultInput.SearchPattern,
            DownloadFromCurrentDirectoryOnly = defaultInput.DownloadFromCurrentDirectoryOnly,
            TargetDirectory = string.Empty,
        };

        var result = await AmazonS3.DownloadObject(input, connection, defaultOptions, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("The value cannot be an empty string."), result.Error.Message);
    }

    [TestMethod]
    public async Task AWSCreds_DownloadFiles_TestAllDestinationFileExistsActions_Test()
    {
        var destinationFileExistsActions = new List<DestinationFileExistsActions>() { DestinationFileExistsActions.Overwrite, DestinationFileExistsActions.Info, DestinationFileExistsActions.Error };

        foreach (var action in destinationFileExistsActions)
        {
            Directory.Delete($@"{dir}\Download", true);

            var connection = new Connection
            {
                AuthenticationMethod = defaultConnection.AuthenticationMethod,
                AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
                AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
                Region = defaultConnection.Region,
                PreSignedUrl = defaultConnection.PreSignedUrl,
            };

            var options = new Options
            {
                ActionOnExistingFile = action,
                DeleteSourceObject = false,
                ThrowErrorIfNoMatch = defaultOptions.ThrowErrorIfNoMatch,
                FileLockedRetries = defaultOptions.FileLockedRetries,
                ThrowErrorOnFailure = defaultOptions.ThrowErrorOnFailure,
                ErrorMessageOnFailure = defaultOptions.ErrorMessageOnFailure,
            };

            var input = new Input
            {
                BucketName = defaultInput.BucketName,
                SourceDirectory = defaultInput.SourceDirectory,
                SearchPattern = defaultInput.SearchPattern,
                DownloadFromCurrentDirectoryOnly = false,
                TargetDirectory = defaultInput.TargetDirectory,
            };

            var result = await AmazonS3.DownloadObject(input, connection, options, CancellationToken.None);
            Assert.IsNotNull(result.Objects, $"method: {action}");
            Assert.IsTrue(result.Success, $"method: {action}");
            Assert.AreEqual(4, result.Objects.Count, $"method: {action}");
            Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null), $"method: {action}");
            Assert.IsTrue(File.Exists(@$"{dir}\Download\DownloadFromCurrentDirectoryOnly.txt"), $"method: {action}");
        }
    }

    [TestMethod]
    public async Task AWSCreds_DownloadFiles_Info_Exists_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = defaultConnection.AuthenticationMethod,
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
            PreSignedUrl = defaultConnection.PreSignedUrl,
        };

        var options = new Options
        {
            ActionOnExistingFile = DestinationFileExistsActions.Info,
            DeleteSourceObject = defaultOptions.DeleteSourceObject,
            ThrowErrorIfNoMatch = defaultOptions.ThrowErrorIfNoMatch,
            FileLockedRetries = defaultOptions.FileLockedRetries,
            ThrowErrorOnFailure = defaultOptions.ThrowErrorOnFailure,
            ErrorMessageOnFailure = defaultOptions.ErrorMessageOnFailure,
        };

        var input = new Input
        {
            BucketName = defaultInput.BucketName,
            SourceDirectory = defaultInput.SourceDirectory,
            SearchPattern = defaultInput.SearchPattern,
            DownloadFromCurrentDirectoryOnly = false,
            TargetDirectory = defaultInput.TargetDirectory,
        };

        var result = await AmazonS3.DownloadObject(input, connection, options, CancellationToken.None);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(4, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Overwrite.txt"));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Testfile.txt"));
        Assert.IsFalse(CompareFiles());
    }

    [TestMethod]
    public async Task AWSCreds_DownloadFiles_Error_Exists_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = defaultConnection.AuthenticationMethod,
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
            PreSignedUrl = defaultConnection.PreSignedUrl,
        };

        var options = new Options
        {
            ActionOnExistingFile = DestinationFileExistsActions.Error,
            DeleteSourceObject = defaultOptions.DeleteSourceObject,
            ThrowErrorIfNoMatch = defaultOptions.ThrowErrorIfNoMatch,
            FileLockedRetries = defaultOptions.FileLockedRetries,
            ThrowErrorOnFailure = defaultOptions.ThrowErrorOnFailure,
            ErrorMessageOnFailure = defaultOptions.ErrorMessageOnFailure,
        };

        var result = await AmazonS3.DownloadObject(defaultInput, connection, options, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("already exists"));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Overwrite.txt"));
        Assert.IsFalse(CompareFiles());
    }

    [TestMethod]
    public async Task AWSCreds_DeleteSource_Test()
    {
        Directory.Delete($@"{dir}\Download", true);
        var connection = new Connection
        {
            AuthenticationMethod = defaultConnection.AuthenticationMethod,
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
            PreSignedUrl = defaultConnection.PreSignedUrl,
        };

        var options = new Options
        {
            DeleteSourceObject = true,
            ActionOnExistingFile = defaultOptions.ActionOnExistingFile,
            ThrowErrorIfNoMatch = defaultOptions.ThrowErrorIfNoMatch,
            FileLockedRetries = defaultOptions.FileLockedRetries,
            ThrowErrorOnFailure = defaultOptions.ThrowErrorOnFailure,
            ErrorMessageOnFailure = defaultOptions.ErrorMessageOnFailure,
        };

        var result = await AmazonS3.DownloadObject(defaultInput, connection, options, CancellationToken.None);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(3, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Overwrite.txt"));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Testfile.txt"));
        Assert.IsFalse(await FileExistsInS3("DownloadTest/testikansio/"));
    }

    [TestMethod]
    public async Task AWSCreds_Pattern_Test()
    {
        Directory.Delete($@"{dir}\Download", true);
        var connection = new Connection
        {
            AuthenticationMethod = defaultConnection.AuthenticationMethod,
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
            PreSignedUrl = defaultConnection.PreSignedUrl,
        };

        var input = new Input
        {
            BucketName = defaultInput.BucketName,
            SourceDirectory = defaultInput.SourceDirectory,
            SearchPattern = "Testfi*",
            DownloadFromCurrentDirectoryOnly = defaultInput.DownloadFromCurrentDirectoryOnly,
            TargetDirectory = defaultInput.TargetDirectory,
        };

        var result = await AmazonS3.DownloadObject(input, connection, defaultOptions, CancellationToken.None);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Testfile.txt"));
        Assert.IsFalse(File.Exists(@$"{dir}\Download\Overwrite.txt"));
        Assert.IsFalse(File.Exists(@$"{dir}\Download\DownloadFromCurrentDirectoryOnly.txt"));
    }

    [TestMethod]
    public async Task AWSCreds_DownloadFromCurrentDirectoryOnly_Test()
    {
        Directory.Delete($@"{dir}\Download", true);
        var connection = new Connection
        {
            AuthenticationMethod = defaultConnection.AuthenticationMethod,
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
            PreSignedUrl = defaultConnection.PreSignedUrl,
        };

        var input = new Input
        {
            BucketName = defaultInput.BucketName,
            SourceDirectory = defaultInput.SourceDirectory,
            SearchPattern = defaultInput.SearchPattern,
            DownloadFromCurrentDirectoryOnly = true,
            TargetDirectory = defaultInput.TargetDirectory,
        };

        var result = await AmazonS3.DownloadObject(input, connection, defaultOptions, CancellationToken.None);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(3, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Overwrite.txt"));
        Assert.IsTrue(File.Exists(@$"{dir}\Download\Testfile.txt"));
        Assert.IsFalse(File.Exists(@$"{dir}\Download\DownloadFromCurrentDirectoryOnly.txt"));
    }

    [TestMethod]
    public async Task AWSCreds_ThrowErrorIfNoMatch_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = defaultConnection.AuthenticationMethod,
            AwsAccessKeyId = defaultConnection.AwsAccessKeyId,
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
            PreSignedUrl = defaultConnection.PreSignedUrl,
        };

        var input = new Input
        {
            BucketName = defaultInput.BucketName,
            SourceDirectory = defaultInput.SourceDirectory,
            SearchPattern = "nofile",
            DownloadFromCurrentDirectoryOnly = defaultInput.DownloadFromCurrentDirectoryOnly,
            TargetDirectory = defaultInput.TargetDirectory,
        };

        var result = await AmazonS3.DownloadObject(input, connection, defaultOptions, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("No matches found with search pattern"));
    }

    [TestMethod]
    public async Task ErrorHandler_ThrowErrorOnFailure_True_WithCustomMessage_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = defaultConnection.AuthenticationMethod,
            AwsAccessKeyId = "invalid",
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
            PreSignedUrl = defaultConnection.PreSignedUrl,
        };

        var options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = "Custom error message for testing",
            ActionOnExistingFile = defaultOptions.ActionOnExistingFile,
            DeleteSourceObject = defaultOptions.DeleteSourceObject,
            ThrowErrorIfNoMatch = defaultOptions.ThrowErrorIfNoMatch,
            FileLockedRetries = defaultOptions.FileLockedRetries,
        };

        try
        {
            await AmazonS3.DownloadObject(defaultInput, connection, options, CancellationToken.None);
            Assert.Fail("Expected exception was not thrown");
        }
        catch (Exception ex)
        {
            Assert.AreEqual("Custom error message for testing", ex.Message);
        }
    }

    [TestMethod]
    public async Task ErrorHandler_ThrowErrorOnFailure_True_WithOriginalMessage_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = defaultConnection.AuthenticationMethod,
            AwsAccessKeyId = "invalid",
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
            PreSignedUrl = defaultConnection.PreSignedUrl,
        };

        var options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = string.Empty,
            ActionOnExistingFile = defaultOptions.ActionOnExistingFile,
            DeleteSourceObject = defaultOptions.DeleteSourceObject,
            ThrowErrorIfNoMatch = defaultOptions.ThrowErrorIfNoMatch,
            FileLockedRetries = defaultOptions.FileLockedRetries,
        };

        try
        {
            await AmazonS3.DownloadObject(defaultInput, connection, options, CancellationToken.None);
            Assert.Fail("Expected exception was not thrown");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("The AWS Access Key Id you provided does not exist") ||
                         ex.Message.Contains("InvalidAccessKeyId") ||
                         ex.Message.Contains("credentials"));
        }
    }

    [TestMethod]
    public async Task ErrorHandler_ThrowErrorOnFailure_False_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = defaultConnection.AuthenticationMethod,
            AwsAccessKeyId = "invalid",
            AwsSecretAccessKey = defaultConnection.AwsSecretAccessKey,
            Region = defaultConnection.Region,
            PreSignedUrl = defaultConnection.PreSignedUrl,
        };

        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = defaultOptions.ErrorMessageOnFailure,
            ActionOnExistingFile = defaultOptions.ActionOnExistingFile,
            DeleteSourceObject = defaultOptions.DeleteSourceObject,
            ThrowErrorIfNoMatch = defaultOptions.ThrowErrorIfNoMatch,
            FileLockedRetries = defaultOptions.FileLockedRetries,
        };

        var result = await AmazonS3.DownloadObject(defaultInput, connection, options, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("The AWS Access Key Id you provided does not exist") ||
                     result.Error.Message.Contains("InvalidAccessKeyId") ||
                     result.Error.Message.Contains("credentials"));
        Assert.IsNotNull(result.Error.AdditionalInfo);
    }

    private string CreatePreSignedUrl(string key)
    {
        AmazonS3Client client = new(accessKey, secretAccessKey, RegionEndpoint.EUCentral1);
        GetPreSignedUrlRequest request = new()
        {
            BucketName = bucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(2),
        };
        return client.GetPreSignedURL(request);
    }

    private async Task CreateTestFiles()
    {
        var file1 = $@"{dir}\Download\Overwrite.txt";
        var file2 = $@"{dir}\DownloadTestFiles\Overwrite.txt";
        var file3 = $@"{dir}\DownloadTestFiles\Testfile.txt";
        var file4 = $@"{dir}\DownloadTestFiles\DownloadFromCurrentDirectoryOnly\DownloadFromCurrentDirectoryOnly.txt";

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var file5 = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Test.pdf");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        var files = new List<string> { file1, file2, file3, file4, file5 };

        Directory.CreateDirectory($@"{dir}\Download");
        Directory.CreateDirectory($@"{dir}\DownloadTestFiles\DownloadFromCurrentDirectoryOnly");
        await File.AppendAllTextAsync(file1, "To Be Overwriten");
        await File.AppendAllTextAsync(file2, $"Overwrite complete {DateTime.UtcNow}");
        await File.AppendAllTextAsync(file3, $"Test {DateTime.UtcNow}");
        await File.AppendAllTextAsync(file4, $"This should exists if DownloadFromCurrentDirectoryOnly = true.  {DateTime.UtcNow}");

        await UploadFileToS3(files);
    }

    private async Task UploadFileToS3(List<string> files)
    {
        var client = new AmazonS3Client(accessKey, secretAccessKey, RegionEndpoint.EUCentral1);

        foreach (var x in files)
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = x.Contains("DownloadFromCurrentDirectoryOnly") ? "DownloadTest/DownloadFromCurrentDirectoryOnly/DownloadFromCurrentDirectoryOnly.txt" : $"DownloadTest/{Path.GetFileName(x)}",
                FilePath = x,
            };
            await client.PutObjectAsync(putObjectRequest);
        }
    }

    private bool CompareFiles()
    {
        string mainFile = File.ReadAllText($@"{dir}\Download\Overwrite.txt");
        return mainFile.Contains("Overwrite complete") && !mainFile.Contains("To Be Overwriten");
    }

    private async Task<bool> FileExistsInS3(string key)
    {
        var client = new AmazonS3Client(accessKey, secretAccessKey, RegionEndpoint.EUCentral1);

        var request = new ListObjectsRequest
        {
            BucketName = bucketName,
            Prefix = key,
        };
        ListObjectsResponse response = await client.ListObjectsAsync(request);
        client.Dispose();
        return response is { S3Objects.Count: > 0 };
    }
}
