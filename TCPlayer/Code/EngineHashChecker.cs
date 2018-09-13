/*
    TC Plyer
    Total Commander Audio Player plugin & standalone player written in C#, based on bass.dll components
    Copyright (C) 2016 Webmaster442 aka. Ruzsinszki Gábor

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace TCPlayer.Code
{
    internal class EngineHashChecker
    {
        private Dictionary<string, string> _storedHashes;

        private string ComputeSha256(string file)
        {
            using (var sha = SHA256.Create())
            {
                using (var fs = File.OpenRead(file))
                {
                    var result = sha.ComputeHash(fs);
                    return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private void GetStoredHashes()
        {
            var currentdir = AppDomain.CurrentDomain.BaseDirectory;
            _storedHashes = new Dictionary<string, string>();
            Assembly assembly = Assembly.GetAssembly(typeof(EngineHashChecker));
            using (var stream = assembly.GetManifestResourceStream("TCPlayer.Engine.Engine.sha256"))
            {
                string line;
                using (var streamreader = new StreamReader(stream))
                {
                    while ((line = streamreader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(' ');
                        var fullpath = Path.Combine(currentdir + @"engine\", parts[1].Replace("*", ""));
                        _storedHashes.Add(fullpath, parts[0]);
                    }
                }
            }
        }

        public EngineHashChecker()
        {
            GetStoredHashes();
        }

        public bool CheckHashes()
        {
            foreach (var hash in _storedHashes)
            {
                if (!File.Exists(hash.Key)) return false;

                var expected = ComputeSha256(hash.Key);

                if (expected != hash.Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
