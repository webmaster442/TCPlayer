using TcPlayer.Engine;

namespace TcPlayer.Tests;

[TestFixture]
internal class EngineFileTests
{
    [Test]
    public void Test_FromFileName_WithVariables_ReturnsCorrect()
    {
        Environment.SetEnvironmentVariable("test", "c:\\testdir");
        var input = "%test%\\test.mp3";

        var result = EngineFile.FromFileName(input);

        Assert.That(result.Uri, Is.EqualTo("file://c:\\testdir\\test.mp3"));
    }
}
