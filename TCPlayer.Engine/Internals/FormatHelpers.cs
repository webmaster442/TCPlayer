using System;
using System.Collections.Generic;
using System.Text;

namespace TCPlayer.Engine.Internals
{
    internal static class FormatHelpers
    {
        /// <summary>
        /// Returns true, if the parameter file is a midi
        /// </summary>
        /// <param name="file">file to check</param>
        /// <returns>true, if midi, false if not</returns>
        public static bool IsMidi(string file)
        {
            string ext = System.IO.Path.GetExtension(file);
            switch (ext)
            {
                case ".midi":
                case ".mid":
                case ".rmi":
                case ".kar":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true, if the parameter file is a tracker format
        /// </summary>
        /// <param name="file">file to check</param>
        /// <returns>true, if tracker, false if not</returns>
        public static bool IsTracker(string file)
        {
            string ext = System.IO.Path.GetExtension(file);
            switch (ext)
            {
                case ".xm":
                case ".it":
                case ".s3m":
                case ".mod":
                case ".mtm":
                case ".umx":
                case ".mo3":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNetwork(string url)
        {
            return url.StartsWith("http://") || url.StartsWith("https://");
        }

        public static bool IsCd(string url)
        {
            return url.StartsWith("cd://");
        }

        public static (int drive, int track) ProcessCdUrl(string cdurl)
        {
            string[] info = cdurl.Replace("cd://", "").Split('/');
            if (info.Length >= 2)
            {
                int.TryParse(info[0], out int drive);
                int.TryParse(info[1], out int track);
                return (drive, track);
            }
            else
            {
                return (-1, -1);
            }
        }
    }
}
