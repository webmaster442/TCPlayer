using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcPlayer.Services;

internal interface IDialogService
{
    string[] OpenFiles();
    void ErrorMessage(string title, string message);
}
