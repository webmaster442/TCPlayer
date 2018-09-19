using System;
using System.Threading;

namespace TaskRunner
{
    public interface IJob<Tinput, Toutput>
    {
        Toutput JobFunction(Tinput inputdata, IProgress<float> progress, CancellationToken ct);
    }

    public interface IJob<TInput>: IJob<TInput, bool>
    {

    }
}
