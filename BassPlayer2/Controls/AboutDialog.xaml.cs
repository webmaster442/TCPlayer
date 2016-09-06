using TCPlayer.Code;
using System;
using System.Windows.Controls;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : UserControl, IDialog
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        public Action OkClicked
        {
            get;
            set;
        }
    }
}
