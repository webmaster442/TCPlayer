using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TCPlayer.Code
{
    internal static class PlaylistLoaders
    {
        public static string[] LoadM3u(string file)
        {
            try
            {
                List<string> ret = new List<string>();
                string filedir = System.IO.Path.GetDirectoryName(file);
                string line;
                using (var content = File.OpenText(file))
                {
                    do
                    {
                        line = content.ReadLine();
                        if (line == null) continue;
                        if (line.StartsWith("#")) continue;
                        if (line.StartsWith("http://") || line.StartsWith("https://"))
                        {
                            ret.Add(line);
                        }
                        else if (line.Contains(":\\") || line.StartsWith("\\\\"))
                        {
                            if (!File.Exists(line)) continue;
                            ret.Add(line);
                        }
                        else
                        {
                            string f = Path.Combine(filedir, line);
                            if (!File.Exists(f)) continue;
                            ret.Add(f);
                        }
                    }
                    while (line != null);
                }
                return ret.ToArray();
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "File Load error");
                return null;
            }
        }

        public static string[] LoadWPL(string file)
        {
            try
            {
                var doc = XDocument.Load(file).Descendants("body").Elements("seq").Elements("media");
                List<string> ret = new List<string>();
                foreach (var media in doc)
                {
                    var src = media.Attribute("src").Value;
                    ret.Add(src);
                }
                return ret.ToArray();
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "File Load error");
                return null;
            }
        }


        public static string[] LoadPls(string file)
        {
            try
            {
                string filedir = System.IO.Path.GetDirectoryName(file);
                List<string> ret = new List<string>();
                string line;
                string pattern = @"^(File)([0-9])+(=)";
                using (var content = File.OpenText(file))
                {
                    do
                    {
                        line = content.ReadLine();
                        if (line == null) continue;
                        if (Regex.IsMatch(line, pattern)) line = Regex.Replace(line, pattern, "");
                        else continue;
                        if (line.StartsWith("http://") || line.StartsWith("https://"))
                        {
                            ret.Add(line);
                        }
                        else if (line.Contains(":\\") || line.StartsWith("\\\\"))
                        {
                            if (!File.Exists(line)) continue;
                            ret.Add(line);
                        }
                        else
                        {
                            string f = Path.Combine(filedir, line);
                            if (!File.Exists(f)) continue;
                            ret.Add(f);
                        }
                    }
                    while (line != null);
                    return ret.ToArray();
                }
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "File Load error");
                return null;
            }
        }
    }
}
