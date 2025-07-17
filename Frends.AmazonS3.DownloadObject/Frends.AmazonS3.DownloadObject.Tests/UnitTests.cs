using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Frends.AmazonS3.DownloadObject.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Frends.AmazonS3.DownloadObject.Tests;

[TestClass]
public class UnitTests
{
    private readonly string? _accessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_AccessKey");
    private readonly string? _secretAccessKey = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_SecretAccessKey");
    private readonly string? _bucketName = Environment.GetEnvironmentVariable("HiQ_AWSS3Test_BucketName");
    private readonly string _dir = Path.Combine(Environment.CurrentDirectory); // .\Frends.AmazonS3.DownloadObject\Frends.AmazonS3.DownloadObject.Test\bin\Debug\net6.0\

    private Connection _connection = new();
    private Input _input = new();
    private Options _options = new();

    [TestInitialize]
    public async Task Initialize()
    {
        _connection = new Connection()
        {
            AuthenticationMethod = AuthenticationMethods.AwsCredentials,
            AwsAccessKeyId = _accessKey,
            AwsSecretAccessKey = _secretAccessKey,
            Region = Region.EuCentral1,
            PreSignedUrl = null,
        };

        _options = new Options()
        {
            DeleteSourceObject = true,
            ThrowErrorIfNoMatch = true,
            ActionOnExistingFile = DestinationFileExistsActions.Overwrite,
            FileLockedRetries = 0,
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = ""
        };

        _input = new Input()
        {
            BucketName = _bucketName,
            SourceDirectory = "DownloadTest/",
            SearchPattern = "*",
            DownloadFromCurrentDirectoryOnly = true,
            TargetDirectory = @$"{_dir}\Download",
        };

        await CreateTestFiles();
    }

    [TestCleanup]
    public void CleanUp()
    {
        Directory.Delete($@"{_dir}\Download", true);
        Directory.Delete($@"{_dir}\DownloadTestFiles", true);
    }

    [TestMethod]
    public async Task PreSignedURL_DownloadFile_Test()
    {
        var setS3Key = $"DownloadTest/Testfile.txt";

        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = CreatePreSignedURL(setS3Key),
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region
        };

        var result = await AmazonS3.DownloadObject(_input, connection, _options, default);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Testfile.txt"));
    }

    [TestMethod]
    public async Task PreSignedURL_NoDestinationFilename_Test()
    {
        var setS3Key = $"DownloadTest/Testfile.txt";

        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = CreatePreSignedURL(setS3Key),
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region
        };

        var result = await AmazonS3.DownloadObject(_input, connection, _options, default);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Testfile.txt"));
    }

    [TestMethod]
    public async Task PreSignedURL_Overwrite_Exists_Test()
    {
        var setS3Key = $"DownloadTest/Overwrite.txt";

        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = CreatePreSignedURL(setS3Key),
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region
        };

        var result = await AmazonS3.DownloadObject(_input, connection, _options, default);
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
            PreSignedUrl = CreatePreSignedURL(setS3Key),
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region
        };
        
        var options = new Options
        {
            ActionOnExistingFile = DestinationFileExistsActions.Info,
            DeleteSourceObject = _options.DeleteSourceObject,
            ThrowErrorIfNoMatch = _options.ThrowErrorIfNoMatch,
            FileLockedRetries = _options.FileLockedRetries,
            ThrowErrorOnFailure = _options.ThrowErrorOnFailure,
            ErrorMessageOnFailure = _options.ErrorMessageOnFailure
        };

        var result = await AmazonS3.DownloadObject(_input, connection, options, default);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(result.Objects.Any(x => x.Info.Contains("Object skipped because file already exists in destination")));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Overwrite.txt"));
        Assert.IsFalse(CompareFiles());
    }

    [TestMethod]
    public async Task PreSignedURL_Error_Exists_Test()
    {
        var setS3Key = $"DownloadTest/Testfile.txt";
        File.WriteAllText(@$"{_dir}\Download\Testfile.txt", "I exist");

        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = CreatePreSignedURL(setS3Key),
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region
        };
        
        var options = new Options
        {
            ActionOnExistingFile = DestinationFileExistsActions.Error,
            DeleteSourceObject = _options.DeleteSourceObject,
            ThrowErrorIfNoMatch = _options.ThrowErrorIfNoMatch,
            FileLockedRetries = _options.FileLockedRetries,
            ThrowErrorOnFailure = _options.ThrowErrorOnFailure,
            ErrorMessageOnFailure = _options.ErrorMessageOnFailure
        };

        var result = await AmazonS3.DownloadObject(_input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("already exists"));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Testfile.txt"));
    }

    [TestMethod]
    public async Task PreSignedURL_MissingURL_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = AuthenticationMethods.PreSignedUrl,
            PreSignedUrl = "",
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region
        };

        var result = await AmazonS3.DownloadObject(_input, connection, _options, default);
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
            PreSignedUrl = CreatePreSignedURL(setS3Key),
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region
        };
        
        var input = new Input
        {
            BucketName = _input.BucketName,
            SourceDirectory = _input.SourceDirectory,
            SearchPattern = _input.SearchPattern,
            DownloadFromCurrentDirectoryOnly = _input.DownloadFromCurrentDirectoryOnly,
            TargetDirectory = ""
        };

        var result = await AmazonS3.DownloadObject(input, connection, _options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("Path cannot be the empty"));
    }

    [TestMethod]
    public async Task AWSCreds_DownloadFiles_TestAllDestinationFileExistsActions_Test()
    {
        var destinationFileExistsActions = new List<DestinationFileExistsActions>() { DestinationFileExistsActions.Overwrite, DestinationFileExistsActions.Info, DestinationFileExistsActions.Error };

        foreach (var action in destinationFileExistsActions)
        {
            Directory.Delete($@"{_dir}\Download", true);

            var connection = new Connection
            {
                AuthenticationMethod = _connection.AuthenticationMethod,
                AwsAccessKeyId = _connection.AwsAccessKeyId,
                AwsSecretAccessKey = _connection.AwsSecretAccessKey,
                Region = _connection.Region,
                PreSignedUrl = _connection.PreSignedUrl
            };
            
            var options = new Options
            {
                ActionOnExistingFile = action,
                DeleteSourceObject = false,
                ThrowErrorIfNoMatch = _options.ThrowErrorIfNoMatch,
                FileLockedRetries = _options.FileLockedRetries,
                ThrowErrorOnFailure = _options.ThrowErrorOnFailure,
                ErrorMessageOnFailure = _options.ErrorMessageOnFailure
            };
            
            var input = new Input
            {
                BucketName = _input.BucketName,
                SourceDirectory = _input.SourceDirectory,
                SearchPattern = _input.SearchPattern,
                DownloadFromCurrentDirectoryOnly = false,
                TargetDirectory = _input.TargetDirectory
            };

            var result = await AmazonS3.DownloadObject(input, connection, options, default);
            Assert.IsNotNull(result.Objects, $"method: {action}");
            Assert.IsTrue(result.Success, $"method: {action}");
            Assert.AreEqual(4, result.Objects.Count, $"method: {action}");
            Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null), $"method: {action}");
            Assert.IsTrue(File.Exists(@$"{_dir}\Download\DownloadFromCurrentDirectoryOnly.txt"), $"method: {action}");
        }
    }

    [TestMethod]
    public async Task AWSCreds_DownloadFiles_Info_Exists_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = _connection.AuthenticationMethod,
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region,
            PreSignedUrl = _connection.PreSignedUrl
        };
        
        var options = new Options
        {
            ActionOnExistingFile = DestinationFileExistsActions.Info,
            DeleteSourceObject = _options.DeleteSourceObject,
            ThrowErrorIfNoMatch = _options.ThrowErrorIfNoMatch,
            FileLockedRetries = _options.FileLockedRetries,
            ThrowErrorOnFailure = _options.ThrowErrorOnFailure,
            ErrorMessageOnFailure = _options.ErrorMessageOnFailure
        };
        
        var input = new Input
        {
            BucketName = _input.BucketName,
            SourceDirectory = _input.SourceDirectory,
            SearchPattern = _input.SearchPattern,
            DownloadFromCurrentDirectoryOnly = false,
            TargetDirectory = _input.TargetDirectory
        };

        var result = await AmazonS3.DownloadObject(input, connection, options, default);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(4, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Overwrite.txt"));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Testfile.txt"));
        Assert.IsFalse(CompareFiles());
    }


    [TestMethod]
    public async Task AWSCreds_DownloadFiles_Error_Exists_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = _connection.AuthenticationMethod,
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region,
            PreSignedUrl = _connection.PreSignedUrl
        };
        
        var options = new Options
        {
            ActionOnExistingFile = DestinationFileExistsActions.Error,
            DeleteSourceObject = _options.DeleteSourceObject,
            ThrowErrorIfNoMatch = _options.ThrowErrorIfNoMatch,
            FileLockedRetries = _options.FileLockedRetries,
            ThrowErrorOnFailure = _options.ThrowErrorOnFailure,
            ErrorMessageOnFailure = _options.ErrorMessageOnFailure
        };

        var result = await AmazonS3.DownloadObject(_input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("already exists"));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Overwrite.txt"));
        Assert.IsFalse(CompareFiles());
    }

    [TestMethod]
    public async Task AWSCreds_DeleteSource_Test()
    {
        Directory.Delete($@"{_dir}\Download", true);
        var connection = new Connection
        {
            AuthenticationMethod = _connection.AuthenticationMethod,
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region,
            PreSignedUrl = _connection.PreSignedUrl
        };
        
        var options = new Options
        {
            DeleteSourceObject = true,
            ActionOnExistingFile = _options.ActionOnExistingFile,
            ThrowErrorIfNoMatch = _options.ThrowErrorIfNoMatch,
            FileLockedRetries = _options.FileLockedRetries,
            ThrowErrorOnFailure = _options.ThrowErrorOnFailure,
            ErrorMessageOnFailure = _options.ErrorMessageOnFailure
        };

        var result = await AmazonS3.DownloadObject(_input, connection, options, default);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(3, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Overwrite.txt"));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Testfile.txt"));
        Assert.IsFalse(await FileExistsInS3("DownloadTest/testikansio/"));
    }

    [TestMethod]
    public async Task AWSCreds_Pattern_Test()
    {
        Directory.Delete($@"{_dir}\Download", true);
        var connection = new Connection
        {
            AuthenticationMethod = _connection.AuthenticationMethod,
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region,
            PreSignedUrl = _connection.PreSignedUrl
        };
        
        var input = new Input
        {
            BucketName = _input.BucketName,
            SourceDirectory = _input.SourceDirectory,
            SearchPattern = "Testfi*",
            DownloadFromCurrentDirectoryOnly = _input.DownloadFromCurrentDirectoryOnly,
            TargetDirectory = _input.TargetDirectory
        };

        var result = await AmazonS3.DownloadObject(input, connection, _options, default);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Testfile.txt"));
        Assert.IsFalse(File.Exists(@$"{_dir}\Download\Overwrite.txt"));
        Assert.IsFalse(File.Exists(@$"{_dir}\Download\DownloadFromCurrentDirectoryOnly.txt"));
    }

    [TestMethod]
    public async Task AWSCreds_DownloadFromCurrentDirectoryOnly_Test()
    {
        Directory.Delete($@"{_dir}\Download", true);
        var connection = new Connection
        {
            AuthenticationMethod = _connection.AuthenticationMethod,
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region,
            PreSignedUrl = _connection.PreSignedUrl
        };
        
        var input = new Input
        {
            BucketName = _input.BucketName,
            SourceDirectory = _input.SourceDirectory,
            SearchPattern = _input.SearchPattern,
            DownloadFromCurrentDirectoryOnly = true,
            TargetDirectory = _input.TargetDirectory
        };

        var result = await AmazonS3.DownloadObject(input, connection, _options, default);
        Assert.IsNotNull(result.Objects);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(3, result.Objects.Count);
        Assert.IsTrue(result.Objects.Any(x => x.ObjectName != null));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Overwrite.txt"));
        Assert.IsTrue(File.Exists(@$"{_dir}\Download\Testfile.txt"));
        Assert.IsFalse(File.Exists(@$"{_dir}\Download\DownloadFromCurrentDirectoryOnly.txt"));
    }

    [TestMethod]
    public async Task AWSCreds_ThrowErrorIfNoMatch_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = _connection.AuthenticationMethod,
            AwsAccessKeyId = _connection.AwsAccessKeyId,
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region,
            PreSignedUrl = _connection.PreSignedUrl
        };
        
        var input = new Input
        {
            BucketName = _input.BucketName,
            SourceDirectory = _input.SourceDirectory,
            SearchPattern = "nofile",
            DownloadFromCurrentDirectoryOnly = _input.DownloadFromCurrentDirectoryOnly,
            TargetDirectory = _input.TargetDirectory
        };

        var result = await AmazonS3.DownloadObject(input, connection, _options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("No matches found with search pattern"));
    }

    [TestMethod]
    public async Task ErrorHandler_ThrowErrorOnFailure_True_WithCustomMessage_Test()
    {
        var connection = new Connection
        {
            AuthenticationMethod = _connection.AuthenticationMethod,
            AwsAccessKeyId = "invalid",
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region,
            PreSignedUrl = _connection.PreSignedUrl
        };
        
        var options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = "Custom error message for testing",
            ActionOnExistingFile = _options.ActionOnExistingFile,
            DeleteSourceObject = _options.DeleteSourceObject,
            ThrowErrorIfNoMatch = _options.ThrowErrorIfNoMatch,
            FileLockedRetries = _options.FileLockedRetries
        };

        try
        {
            await AmazonS3.DownloadObject(_input, connection, options, default);
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
            AuthenticationMethod = _connection.AuthenticationMethod,
            AwsAccessKeyId = "invalid",
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region,
            PreSignedUrl = _connection.PreSignedUrl
        };
        
        var options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = "",
            ActionOnExistingFile = _options.ActionOnExistingFile,
            DeleteSourceObject = _options.DeleteSourceObject,
            ThrowErrorIfNoMatch = _options.ThrowErrorIfNoMatch,
            FileLockedRetries = _options.FileLockedRetries
        };

        try
        {
            await AmazonS3.DownloadObject(_input, connection, options, default);
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
            AuthenticationMethod = _connection.AuthenticationMethod,
            AwsAccessKeyId = "invalid",
            AwsSecretAccessKey = _connection.AwsSecretAccessKey,
            Region = _connection.Region,
            PreSignedUrl = _connection.PreSignedUrl
        };
        
        var options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = _options.ErrorMessageOnFailure,
            ActionOnExistingFile = _options.ActionOnExistingFile,
            DeleteSourceObject = _options.DeleteSourceObject,
            ThrowErrorIfNoMatch = _options.ThrowErrorIfNoMatch,
            FileLockedRetries = _options.FileLockedRetries
        };

        var result = await AmazonS3.DownloadObject(_input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("The AWS Access Key Id you provided does not exist") || 
                     result.Error.Message.Contains("InvalidAccessKeyId") ||
                     result.Error.Message.Contains("credentials"));
        Assert.IsNotNull(result.Error.AdditionalInfo);
    }

    private string CreatePreSignedURL(string key)
    {
        AmazonS3Client? client = new(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);
        GetPreSignedUrlRequest request = new()
        {
            BucketName = _bucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(2)
        };
        return client.GetPreSignedURL(request);
    }

    private async Task<bool> CreateTestFiles()
    {
        var file1 = $@"{_dir}\Download\Overwrite.txt";
        var file2 = $@"{_dir}\DownloadTestFiles\Overwrite.txt";
        var file3 = $@"{_dir}\DownloadTestFiles\Testfile.txt";
        var file4 = $@"{_dir}\DownloadTestFiles\DownloadFromCurrentDirectoryOnly\DownloadFromCurrentDirectoryOnly.txt";

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var file5 = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Test.pdf");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        var files = new List<string> { file1, file2, file3, file4, file5 };

        Directory.CreateDirectory($@"{_dir}\Download");
        Directory.CreateDirectory($@"{_dir}\DownloadTestFiles\DownloadFromCurrentDirectoryOnly");
        File.AppendAllText(file1, "To Be Overwriten");
        File.AppendAllText(file2, $"Overwrite complete {DateTime.UtcNow}");
        File.AppendAllText(file3, $"Test {DateTime.UtcNow}");
        File.AppendAllText(file4, $"This should exists if DownloadFromCurrentDirectoryOnly = true.  {DateTime.UtcNow}");

        return await UploadFileToS3(files);
    }

    private async Task<bool> UploadFileToS3(List<string> files)
    {
        var client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);

        foreach (var x in files)
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = x.Contains("DownloadFromCurrentDirectoryOnly") ? "DownloadTest/DownloadFromCurrentDirectoryOnly/DownloadFromCurrentDirectoryOnly.txt" : $"DownloadTest/{Path.GetFileName(x)}",
                FilePath = x,
            };
            await client.PutObjectAsync(putObjectRequest);
        }
        return true;
    }

    private bool CompareFiles()
    {
        string mainFile = File.ReadAllText($@"{_dir}\Download\Overwrite.txt");
        return mainFile.Contains("Overwrite complete") && !mainFile.Contains("To Be Overwriten");
    }

    private async Task<bool> FileExistsInS3(string key)
    {
        var client = new AmazonS3Client(_accessKey, _secretAccessKey, RegionEndpoint.EUCentral1);

        var request = new ListObjectsRequest
        {
            BucketName = _bucketName,
            Prefix = key,
        };
        ListObjectsResponse response = await client.ListObjectsAsync(request);
        client.Dispose();
        return (response != null && response.S3Objects != null && response.S3Objects.Count > 0);
    }
}
