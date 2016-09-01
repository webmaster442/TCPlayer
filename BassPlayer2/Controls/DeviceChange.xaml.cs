using BassPlayer2.Code;
using System;
using System.Windows.Controls;

namespace BassPlayer2.Controls
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
