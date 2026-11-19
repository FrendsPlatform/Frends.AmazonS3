using Frends.AmazonS3.UploadObject.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.AmazonS3.UploadObject.Tests;

[TestClass]
public class OptionsTests
{
    [TestMethod]
    public void ThrowErrorIfNoMatch_IsEnabledByDefault()
    {
        Assert.IsTrue(new Options().ThrowErrorIfNoMatch);
    }
}
