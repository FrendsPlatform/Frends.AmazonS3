using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AmazonS3.UploadObject.Tests;

/// <summary>
/// Guards against the "task stuck forever" bug: when the source file
/// stays locked (e.g. held open by another process/AV scan), the private DeleteSourceFile /
/// IsFileLocked retry loop in UploadObject.cs used to have no timeout, no max attempt count and
/// did not observe a CancellationToken, so it spun on Thread.Sleep(1000) indefinitely.
/// This test does not touch S3 at all - it calls the private method directly via reflection
/// so it can prove the fix (or reproduce the bug, if regressed) in seconds instead of 11 hours.
/// </summary>
[TestClass]
public class DeleteSourceFileHangTests
{
    private string dir = null!;
    private string filePath = null!;

    [TestInitialize]
    public void Initialize()
    {
        dir = Path.Combine(Environment.CurrentDirectory, "HangTest");
        Directory.CreateDirectory(dir);
        filePath = Path.Combine(dir, "locked.txt");
        File.WriteAllText(filePath, "locked content");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(dir))
            Directory.Delete(dir, true);
    }

    private static MethodInfo GetDeleteSourceFileMethod()
    {
        var method = typeof(AmazonS3).GetMethod(
            "DeleteSourceFile",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method, "Could not find private method DeleteSourceFile via reflection.");

        return method;
    }

    [TestMethod]
    [Timeout(15000)]
    public async Task DeleteSourceFile_DoesNotHangForever_WhenFileStaysLocked()
    {
        await using var lockingStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        var deleteTask = (Task)GetDeleteSourceFileMethod().Invoke(null, [filePath, CancellationToken.None])!;
        var completedTask = await Task.WhenAny(deleteTask, Task.Delay(TimeSpan.FromSeconds(10)));
        Assert.AreSame(deleteTask, completedTask,
            "DeleteSourceFile did not return within 10 seconds while the source file was locked.");
        Exception? thrown = null;

        try
        {
            await deleteTask;
        }
        catch (Exception ex)
        {
            thrown = ex;
        }

        Assert.IsNotNull(thrown,
            "Expected DeleteSourceFile to throw after exhausting its retry attempts because the file remained locked for the entire wait window.");
    }

    [TestMethod]
    [Timeout(15000)]
    public async Task DeleteSourceFile_ObservesCancellationToken_WhenFileStaysLocked()
    {
        await using var lockingStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        using var cts = new CancellationTokenSource();

        var deleteTask = (Task)GetDeleteSourceFileMethod()
            .Invoke(null, [filePath, cts.Token])!;

        await cts.CancelAsync();

        var completedTask = await Task.WhenAny(deleteTask, Task.Delay(TimeSpan.FromSeconds(10)));

        Assert.AreSame(deleteTask, completedTask,
            "DeleteSourceFile did not react to a cancelled CancellationToken within 10 seconds " +
            "while the source file was locked.");

        await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => deleteTask);
    }

    [TestMethod]
    public async Task DeleteSourceFile_DeletesFile_WhenNotLocked()
    {
        var deleteTask = (Task)GetDeleteSourceFileMethod()
            .Invoke(null, [filePath, CancellationToken.None])!;

        await deleteTask;

        Assert.IsFalse(File.Exists(filePath));
    }
}
