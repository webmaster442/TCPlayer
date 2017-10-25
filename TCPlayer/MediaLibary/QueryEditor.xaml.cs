using System;
using System.Collections.Generic;
using System.Windows.Controls;
using TCPlayer.Code;
using TCPlayer.MediaLibary.DB;

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
                    { ButtonNames.BtnSave.ToString(), "Save" },
                    { ButtonNames.BtnExec.ToString(), "Execute" },
                    { ButtonNames.BtnSaveExec.ToString(), "Save & Execute" },
                };
            }
        }

        public QueryInput CurrentQuery
        {
            get
            {
                if (DataContext != null)
                    return (DataContext as QueryInput);
                else
                    return null;
            }
        }

        public enum ButtonNames
        {
            BtnSave,
            BtnExec,
            BtnSaveExec
        }

        public Action<string> ButtonClickHandler
        {
            get;
            set;
        }
    }
}
