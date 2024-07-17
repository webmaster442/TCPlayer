using System.Security.Cryptography.X509Certificates;

using TcPlayer.Engine.Formats.Cue;
using TcPlayer.Engine;

namespace TcPlayer.Tests;

[TestFixture]
internal class CueParserTests
{
    const string RekordboxCue = """
        REM DATE 2024-05-05 09:36 PM
        REM RECORDED_BY "rekordbox-dj"
        TITLE "REC-2024-05-05"
        PERFORMER "TNNR"
        FILE "01 REC-2024-05-05.wav" WAVE
        	TRACK 01 AUDIO
        		TITLE "Drop Em Down (Original Mix)"
        		PERFORMER "D-Sturb And Malice"
        		FILE "D:/0Zene/HardStyle/Malice - The Extreme/15 D-Sturb And Malice - Drop Em Down (Original Mix).m4a" WAVE
        		INDEX 01 00:00:00
        	TRACK 02 AUDIO
        		TITLE "LEVITATE (Extended Mix)"
        		PERFORMER "Warface & Adjuzt Feat Iris Goes"
        		FILE "D:/0Zene/HardStyle/Warface - Rest In Pieces Extended/02-warface_and_adjuzt_feat_iris_goes_-_levitate_(extended_mix).mp3" WAVE
        		INDEX 01 00:02:45
        	TRACK 03 AUDIO
        		TITLE "Force Of Will (Extended Mix)"
        		PERFORMER "Unresolved & Aversion"
        		FILE "D:/0Zene/Vinils/Hardstyle/Unresolved & Aversion - Force Of Will (Extended Mix).mp3" WAVE
        		INDEX 01 00:05:58
        	TRACK 04 AUDIO
        		TITLE "Disruption (Playground 03 OST) (Extended Mix)"
        		PERFORMER "D-Sturb"
        		FILE "D:/zenele/d-surb/03_d-sturb_-_disruption_(playground_03_ost)_(extended_mix).flac" WAVE
        		INDEX 01 00:09:19
        	TRACK 05 AUDIO
        		TITLE "Impact (Playground 01 OST) (Extended Mix)"
        		PERFORMER "D-Sturb"
        		FILE "D:/0Zene/Vinils/Hardstyle/D-Sturb - Impact (Playground 01 OST) (Extended Mix).mp3" WAVE
        		INDEX 01 00:13:12
        	TRACK 06 AUDIO
        		TITLE "For The Night"
        		PERFORMER "Warface & D-Sturb"
        		FILE "D:/0Zene/HardStyle/Warface And D-Sturb - Synchronised/05-warface_and_d-sturb_-_for_the_night.mp3" WAVE
        		INDEX 01 00:16:42
        	TRACK 07 AUDIO
        		TITLE "Darkside"
        		PERFORMER "Aversion & Unresolved"
        		FILE "D:/0Zene/Vinils/Hardstyle/Aversion & Unresolved - Darkside.mp3" WAVE
        		INDEX 01 00:21:04
        	TRACK 08 AUDIO
        		TITLE "Die Gladiator [Rebelión Extended RMX]"
        		PERFORMER "Endymion"
        		FILE "D:/0Zene/HardStyle/Rebelion - The First Dose/01 Endymion - Die Gladiator (Rebelion Remix).mp3" WAVE
        		INDEX 01 00:24:02
        	TRACK 09 AUDIO
        		TITLE "Mutation (Pro Mix)"
        		PERFORMER "Riot Shift"
        		FILE "D:/0Zene/Vinils/Hardstyle/Riot Shift - Mutation (Pro Mix).m4a" WAVE
        		INDEX 01 00:25:34
        	TRACK 10 AUDIO
        		TITLE "White_noise (Extended Mix)"
        		PERFORMER "Cybergore"
        		FILE "D:/zenele/mx/01_cybergore_-_white_noise_(extended_mix).flac" WAVE
        		INDEX 01 00:28:41
        	TRACK 11 AUDIO
        		TITLE "NITELIFE (Pro Mix)"
        		PERFORMER "Riot Shift"
        		FILE "D:/0Zene/Vinils/Hardstyle/Riot Shift - NITELIFE (Pro Mix).mp3" WAVE
        		INDEX 01 00:31:03
        	TRACK 12 AUDIO
        		TITLE "666 (Extended Mix)"
        		PERFORMER "Riot Shift & So Juice"
        		FILE "D:/0Zene/Vinils/Hardstyle/Riot Shift & So Juice - 666 (Extended Mix).mp3" WAVE
        		INDEX 01 00:33:57
        	TRACK 13 AUDIO
        		TITLE "Slow Death (The Smiler Remix) (Pro Mix)"
        		PERFORMER "Kruelty"
        		FILE "D:/zenele/hs/01_kruelty_-_slow_death_(the_smiler_remix)_(pro_mix).flac" WAVE
        		INDEX 01 00:36:24
        
        """;

    [Test]
    public void Test_RekordboxParse()
    {
        var chapters = CueParser.Parse(RekordboxCue.Split('\n')).ToList();
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
