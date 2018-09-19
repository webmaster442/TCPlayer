using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace TaskRunner
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class JobWindow : Window
    {
        private CancellationTokenSource _tokensource;
        private Progress<float> _progressReporter;
        private bool _reportTaskBarProgress;

        private JobWindow()
        {
            InitializeComponent();
            _tokensource = new CancellationTokenSource();
            _progressReporter = new Progress<float>();
            _progressReporter.ProgressChanged += _progressReporter_ProgressChanged;
        }

        public JobWindow(string jobTitle, string jobDescription, bool reportTaskBarProgress) : this()
        {
            Title = jobTitle;
            Description.Text = jobDescription;
            _reportTaskBarProgress = reportTaskBarProgress;
        }

        public IProgress<float> Reporter
        {
            get { return _progressReporter; }
        }

        public CancellationToken CancelToken
        {
            get { return _tokensource.Token; }
        }

        private void _progressReporter_ProgressChanged(object sender, float e)
        {
            Dispatcher.Invoke(() =>
             {
                 if (_reportTaskBarProgress)
                 {
                     if (TaskBar.ProgressState != TaskbarItemProgressState.Normal)
                     {
                         TaskBar.ProgressState = TaskbarItemProgressState.Normal;
                     }
                     TaskBar.ProgressValue = (double)e;
                 }
                 ProgressBar.Value = e * 100.0d;
             });
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_reportTaskBarProgress)
            {
                TaskBar.ProgressState = TaskbarItemProgressState.Paused;
            }
            _tokensource.Cancel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_reportTaskBarProgress)
            {
                TaskBar.ProgressState = TaskbarItemProgressState.Paused;
            }
            _tokensource.Cancel();
        }
    }
}
