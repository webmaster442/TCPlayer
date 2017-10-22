using System;
using System.Windows.Controls;
using TCPlayer.Code;

namespace TCPlayer.MediaLibary
{
    /// <summary>
    /// Interaction logic for QueryEditor.xaml
    /// </summary>
    public partial class QueryEditor : UserControl, IDialog
    {
        public QueryEditor()
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
