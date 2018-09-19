using System.Windows;

namespace TaskRunner
{
    public class JobRunnerConfiguration<Tinput, Toutput>
    {
        public IJob<Tinput, Toutput> Job { get; set; }
        public Tinput JobInput { get; set; }
        public bool ReportTaskBarProgress { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
    }
}
