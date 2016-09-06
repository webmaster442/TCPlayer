using TCPlayer.Code;
using System;
using System.Windows.Controls;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for AddURLDialog.xaml
    /// </summary>
    public partial class AddURLDialog : UserControl, IDialog
    {
        public AddURLDialog()
        {
            InitializeComponent();
        }

        public Action OkClicked
        {
            get; set;
        }

        public string Url
        {
            get { return TbUrl.Text; }
        }
    }
}
