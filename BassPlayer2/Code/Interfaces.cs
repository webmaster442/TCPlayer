using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BassPlayer2.Code
{
    public interface IDialog
    {
        Action OkClicked { get; set; }
    }
}
