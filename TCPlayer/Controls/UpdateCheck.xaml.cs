using System;
using System.Net;
using System.Windows;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for UpdateCheck.xaml
    /// </summary>
    public partial class UpdateCheck : Window
    {
        private const string UpdatePathUrl = "";
        private const int UpdateCheckDays = 7;

        public UpdateCheck(Version v)
        {
            InitializeComponent();
            Header.Text = string.Format(Properties.Resources.UpdateCheck_Header, v);
        }

        public static async void CheckForUpdate()
        {
            if (!Properties.Settings.Default.CheckForUpdates)
                return;

            var lastCheck = DateTime.UtcNow - Properties.Settings.Default.LastUpdateCheck;

            if (lastCheck < TimeSpan.FromDays(UpdateCheckDays))
                return;

            try
            {
                using (WebClient client = new WebClient())
                {
                    string versionString = await client.DownloadStringTaskAsync(UpdatePathUrl);
                    if (Version.TryParse(versionString, out Version parsed))
                    {
                        if (parsed > GetCurrentVersion())
                        {
                            var notification = new UpdateCheck(parsed);
                            notification.ShowDialog();
                            Properties.Settings.Default.LastUpdateCheck = DateTime.UtcNow;
                            Properties.Settings.Default.Save();
                        }
                    }
                }
            }
            catch (WebException)
            {
                MessageBox.Show(Properties.Resources.UpdateCheck_Error, 
                                Properties.Resources.Error_Title,
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
            }
        }

        private static Version GetCurrentVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
