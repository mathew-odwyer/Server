using System;
using NUnit.Framework;

namespace Winterhaven.Common.Tests;

[TestFixture]
internal sealed class BuildInformationTests
{
    [Test]
    public void VersionShouldReturnValidSemVer()
    {
        string version = BuildInformation.Version;
        Assert.DoesNotThrow(() => Version.Parse(version));
    }
}
