using Frends.AmazonS3.DeleteBucket.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.AmazonS3.DeleteBucket.Tests;

[TestClass]
public class OptionsTests
{
    [TestMethod]
    public void ThrowErrorOnFailure_IsEnabledByDefault()
    {
        Assert.IsTrue(new Options().ThrowErrorOnFailure);
    }
}
