using BassPlayer2.Code;
using System;
using System.Windows.Controls;

namespace BassPlayer2.Controls
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
