using System.Security.Cryptography.X509Certificates;

using TcPlayer.Engine.Formats.Cue;
using TcPlayer.Engine;

namespace TcPlayer.Tests;

[TestFixture]
internal class CueParserTests
{
    [Test]
    public void Test_RekordboxParse()
    {
        var chapters = CueParser.Parse(TestData.RekordboxCue.Split('\n')).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(chapters, Has.Count.EqualTo(13));
            Assert.That(chapters[12], Is.EqualTo(new Chapter
            {
                Title = "Kruelty - Slow Death (The Smiler Remix) (Pro Mix)",
                StartTimeSeconds = 2184,
            }));
        });
    }
}
