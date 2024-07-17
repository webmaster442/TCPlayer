namespace TcPlayer.Engine;

public interface IPlaylist
{
    int CurrentIndex { get; set; }
    int Count { get; }
    EngineFile this[int index] { get; }
    //void ResetCurrentIndex();
}