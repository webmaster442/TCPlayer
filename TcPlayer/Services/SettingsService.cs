namespace TcPlayer.Services;

internal class SettingsService : ISettingsService
{
    public int SelectedAudioOutput
    {
        get => Properties.Settings.Default.SelectedAudioOutput;
        set
        {
            Properties.Settings.Default.SelectedAudioOutput = value;
            Properties.Settings.Default.Save();
        }
    }

    public float VolumeLevel
    {
        get => Properties.Settings.Default.VolumeLevel;
        set
        {
            Properties.Settings.Default.VolumeLevel = value;
            Properties.Settings.Default.Save();
        }
    }
}