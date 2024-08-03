namespace TcPlayer.Services;
internal interface ISettingsService
{
    int SelectedAudioOutput { get; set; }
    float VolumeLevel { get; set; }
}
