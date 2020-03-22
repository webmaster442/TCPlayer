using System;
using System.Collections.Generic;
using System.Text;

namespace TCPlayer.Engine
{
    public interface IEngineConfiguration
    {
        string SoundFontPath { get; set; }
        int LastDeviceId { get; set; }
        int Frequency { get; set; }
        bool RememberDeviceId { get; set; }
    }
}
