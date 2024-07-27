using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace TcPlayer;
public abstract class SingleInstanceApp : Application, IDisposable
{
    /// <summary>
    /// Application id. Must be unique
    /// </summary>
    public string? AppId { get; set; }

    private Mutex? _appLockMutex;
    private Thread? _namedPipeServer;
    private NamedPipeServerStream? _namedPipeServerStream;
    private CancellationTokenSource? _cancellationTokenSource;


    /// <summary>
    /// Startup argument handling. Executed when startup and when an other instance is started
    /// </summary>
    /// <param name="args">startup arguments</param>
    public abstract void HandleArguments(string[] args);

    protected override void OnStartup(StartupEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(AppId))
            throw new InvalidOperationException("AppId is not set");

        string pipeName = $"{AppId}/pipe";

        base.OnStartup(e);
        if (Mutex.TryOpenExisting(AppId, out Mutex? capturedMutex))
        {
            var argumentsJson = JsonSerializer.Serialize(e.Args);
            using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
            {
                client.Connect();
                using (var writer = new StreamWriter(client, leaveOpen: true))
                {
                    writer.WriteLine(argumentsJson);
                }
                client.Close();
                Shutdown();
            }
            capturedMutex.Close();
        }
        else
        {
            _appLockMutex = new Mutex(false, AppId);
            _namedPipeServer = new Thread(NamedPipeListener);
            _namedPipeServerStream = new NamedPipeServerStream(pipeName, PipeDirection.In);
            _cancellationTokenSource = new CancellationTokenSource();
            _namedPipeServer.Start(_cancellationTokenSource.Token);
            HandleArguments(e.Args);
        }
    }

    private void NamedPipeListener(object? obj)
    {
        if (_namedPipeServerStream == null)
            throw new InvalidOperationException("Incorrect pipe setup");

        var token = (CancellationToken)obj!;
        while (!token.IsCancellationRequested)
        {
            _namedPipeServerStream.WaitForConnection();
            using (var reader = new StreamReader(_namedPipeServerStream, leaveOpen: true))
            {
                var json = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                {
                    var args = JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
                    Dispatcher.Invoke(() => HandleArguments(args), DispatcherPriority.Input);
                }
            }
            _namedPipeServerStream.Disconnect();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_namedPipeServer != null)
        {
            _cancellationTokenSource?.Cancel();
            string pipeName = $"{AppId}/pipe";
            using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
            {
                client.Connect();
            }
            Thread.Sleep(100);
        }
        _namedPipeServerStream?.Dispose();
        _appLockMutex?.Dispose();
        _cancellationTokenSource?.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}