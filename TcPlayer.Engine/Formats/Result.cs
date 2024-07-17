using System.Diagnostics;

namespace TcPlayer.Engine.Formats;

public class Result<T>
{
    private readonly T? _result;
    private readonly Exception? _exception;

    public Result(T result)
    {
        _result = result;
    }

    public Result(Exception exception) 
    {
        _exception = exception;
    }

    public void Handle(Action<T> ok, Action<Exception> exception)
    {
        if (_result != null)
        {
            ok(_result);
            return;
        }
        else if (_exception != null)
        {
            exception(_exception);
            return;
        }
        throw new UnreachableException();
    }
}
