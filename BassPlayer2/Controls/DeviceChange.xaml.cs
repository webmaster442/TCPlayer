using TCPlayer.Code;
using System;
using System.Windows.Controls;

namespace TCPlayer.Controls
{
    /// <summary>
    /// Interaction logic for DeviceChange.xaml
    /// </summary>
    public partial class DeviceChange : UserControl, IDialog
    {
        public DeviceChange()
        {
            InitializeComponent();
        }

        public Action OkClicked
        {
            get; set;
        }

        public int DeviceIndex
        {
            get
            {
                return LbDevices.SelectedIndex;
            }
        }
    }
}
