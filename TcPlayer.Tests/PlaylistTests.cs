using TcPlayer.Engine.Formats;

namespace TcPlayer.Tests;

[TestFixture]
internal class PlaylistTests
{
    [SetUp]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("test", "c:\\testdir");
    }

    [TearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable("test", null);
    }

    [Test]
    public async Task Test_LoadM3U_ReturnsCorrect()
    {
        using var data = new StringReader(TestData.M3UFile);
        var result = await Playlists.LoadM3U(data, "c:\\test.m3u");
        result.Handle((list) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(list.Count, Is.EqualTo(4));
                Assert.That(list[0].Path, Is.EqualTo("C:\\Documents and Settings\\I\\My Music\\Sample.mp3"));
                Assert.That(list[2].Path, Is.EqualTo("c:\\testdir\\Music\\short.ogg"));
                Assert.That(list[3].Path, Is.EqualTo("c:\\example2.mp3"));

            });
        },
        (e) => Assert.Fail(e.Message));
    }

    [Test]
    public async Task Test_LoadPls_ReturnsCorrect()
    {
        using var data = new StringReader(TestData.PlsWithLocalFiles);
        var result = await Playlists.LoadPls(data, "c:\\test.pls");
        result.Handle((list) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0].Path, Is.EqualTo("c:\\example2.mp3"));
                Assert.That(list[1].Path, Is.EqualTo("F:\\Music\\whatever.m4a"));
                Assert.That(list[2].Path, Is.EqualTo("c:\\testdir\\Music\\short.ogg"));
            });
        },
        (e) => Assert.Fail(e.Message));
    }
}
