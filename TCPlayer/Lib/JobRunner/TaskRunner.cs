using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace TaskRunner
{
    public static class JobRunner
    {
        public static async Task<JobResult<Toutput>> RunJob<Tinput, Toutput>(JobRunnerConfiguration<Tinput, Toutput> configuration)
        {
            if (configuration == null)
                throw new NullReferenceException(nameof(configuration));

            JobWindow tw = new JobWindow(configuration.JobTitle, configuration.JobDescription, configuration.ReportTaskBarProgress);
            try
            {
                Toutput result = default(Toutput);
                Application.Current.Dispatcher.Invoke(() => tw.Show());
                result = await Task.Run(() => configuration.Job.JobFunction(configuration.JobInput, tw.Reporter, tw.CancelToken));
                Application.Current.Dispatcher.Invoke(() => tw.Close());
                return JobResult<Toutput>.Create(result, true);
            }
            catch (OperationCanceledException ex)
            {
                tw.Close();
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("Task Cancelced: {0}", ex);
                }
                return JobResult<Toutput>.CanceledJob();
            }
        }
    }
}
