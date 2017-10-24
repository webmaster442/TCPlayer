using System;
using System.Collections.Generic;
using System.Windows.Controls;
using TCPlayer.Code;

namespace TCPlayer.MediaLibary
{
    /// <summary>
    /// Interaction logic for QueryEditor.xaml
    /// </summary>
    public partial class QueryEditor : UserControl, IDialogWithCustomButtons
    {
        public QueryEditor()
        {
            InitializeComponent();
        }

        public IDictionary<string, string> ButtonContents
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "BtnSave", "Save" },
                    { "BtnExec", "Execute" },
                    { "BtnSaveExec", "Save & Execute" },
                };
            }
        }

        public Action<string> ButtonClickHandler
        {
            get;
            set;
        }
    }
}
