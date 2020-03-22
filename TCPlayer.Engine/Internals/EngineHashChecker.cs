using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace TCPlayer.Engine.Internals
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
