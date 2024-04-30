using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TcPlayer.Engine;
using TcPlayer.Engine.Notifications;
using TcPlayer.Services;

namespace TcPlayer.ViewModels;

internal partial class PlayerControlsViewModel : 
    ObservableObject, 
    IMessageClient<EngineLoadNotification>,
    IMessageClient<EngineStateChangeNotification>
{
    private readonly IEngine _engine;
    private readonly IDialogService _dialogService;
    
    [ObservableProperty]
    private double _currentPosition;

    [ObservableProperty]
    private double _totalTime;

    [ObservableProperty]
    private double _volume;

    public double Remaining => TotalTime - CurrentPosition;

    [ObservableProperty]
    private DeviceInfo _selectedDevice;

    partial void OnSelectedDeviceChanged(DeviceInfo value)
        => _engine.Init(value);

    public ObservableCollection<DeviceInfo> Devices { get; }


    public PlayerControlsViewModel(IEngine engine,
                                   IDialogService dialogService,
                                   IMediator mediator)
    {
        mediator.Register(this);
        _engine = engine;
        _dialogService = dialogService;
        Devices = new ObservableCollection<DeviceInfo>(_engine.GetDevices());
        if (Devices.Count < 1)
        {
            _dialogService.ErrorMessage("No sound card", "No sound output detected, Program will exit");
            Environment.Exit(-1);
        }
        SelectedDevice = Devices[0];

        //TODO: remove when app finished
        {
            _engine.Load(EngineFile.FromFileName(@"d:\Lazy Laura - Quincas Moreira.mp3"));
            _engine.Pause();
        }
    }

    [RelayCommand]
    public void PlayPause()
    {

    }

    void IMessageClient<EngineLoadNotification>.OnNotify(EngineLoadNotification message)
    {
        //throw new NotImplementedException();
    }

    void IMessageClient<EngineStateChangeNotification>.OnNotify(EngineStateChangeNotification message)
    {
        CurrentPosition = message.PositionSeconds;
        TotalTime = message.LengthSeconds;
        OnPropertyChanged(nameof(Remaining));
        Volume = message.Volume;
    }
}
