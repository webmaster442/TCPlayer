using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskRunner
{
    public static class JobRunner
    {
        public static async Task<JobResult<Toutput>> RunJob<Tinput, Toutput>(JobRunnerConfiguration<Tinput, Toutput> configuration)
        {
            try
            {
                JobWindow tw = new JobWindow(configuration.JobTitle, configuration.JobDescription, configuration.ReportTaskBarProgress);
                Toutput result = default(Toutput);
                tw.Show();
                result = await Task.Run(() => configuration.Job.JobFunction(configuration.JobInput, tw.Reporter, tw.CancelToken));
                tw.Close();
                return JobResult<Toutput>.Create(result, true);
            }
            catch (TaskCanceledException ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("Task Cancelced: {0}", ex);
                }
                return JobResult<Toutput>.CanceledJob();
            }
        }
    }
}
