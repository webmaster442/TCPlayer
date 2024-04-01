// --------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This software is licensed under the MIT license. See LICENSE file for details.
// --------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Linq;

namespace TCPluginInstaller.Logic
{
    internal static class TCPluginInstaller
    {
        private const string WlxFile = "TCPlayerLister.wlx";
        private const string WcxFile = "TCPlayerPacker.wcx";

        public static void Install(string file, bool lister, bool packer)
        {
            string iniFile = file;
            string currentdir = AppDomain.CurrentDomain.BaseDirectory;
            string section = "";
            string fullpath = "";

            if (lister)
            {
                section = ToString(PluginType.Lister);
                fullpath = Path.Combine(currentdir, WlxFile);
                var keys = IniFile.GetKeyValuePairs(iniFile, section);
                int index = 0;
                if (keys != null)
                {
                    index = keys.Keys.Select(k => Convert.ToInt32(k)).Max();
                    index++;
                }
                IniFile.WriteValue(iniFile, section, index.ToString(), fullpath);
            }

            if (packer)
            {
                section = ToString(PluginType.Packer);
                fullpath = Path.Combine(currentdir, WcxFile);
                IniFile.WriteValue(iniFile, section, "tcplayer", $"277,{fullpath}");
            }
        }

        private static string ToString(PluginType pluginType)
        {
            switch (pluginType)
            {
                case PluginType.Packer:
                    return "PackerPlugins";
                case PluginType.Lister:
                    return "ListerPlugins";
                default:
                    throw new InvalidOperationException("Unknown type");
            }
        }
    }
}
