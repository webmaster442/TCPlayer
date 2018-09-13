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
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace TCPlayer.Code
{
    //based on https://weblog.west-wind.com/posts/2016/May/13/Creating-Single-Instance-WPF-Applications-that-open-multiple-Files
    public class SingleInstanceApp
    {
        private const string EXIT_STRING = "__EXIT__";
        private static Mutex _mutex;
        private bool _isfirst;
        private bool _isRunning = false;
        private Thread _server;
        private string _UID;

        private static string GetUnique(string AppName)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(AppName);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        private void ServerThread()
        {
            while (true)
            {
                string text;
                using (var server = new NamedPipeServerStream(_UID))
                {
                    server.WaitForConnection();

                    using (StreamReader reader = new StreamReader(server))
                    {
                        text = reader.ReadToEnd();
                    }
                }

                if (text == EXIT_STRING) break;

                ReceiveString?.Invoke(text);
                if (_isRunning == false) break;
            }
        }

        private bool Write(string text, int connectTimeout = 300)
        {
            using (var client = new NamedPipeClientStream(_UID))
            {
                try { client.Connect(connectTimeout); }
                catch { return false; }

                if (!client.IsConnected) return false;

                using (StreamWriter writer = new StreamWriter(client))
                {
                    writer.Write(text);
                    writer.Flush();
                }
            }
            return true;
        }

        public event Action<string> ReceiveString;

        public SingleInstanceApp(string AppName)
        {
            _UID = GetUnique(AppName);
            _mutex = new Mutex(true, _UID, out _isfirst);
            _server = new Thread(ServerThread);
            _isRunning = true;
            _server.Start();
        }

        public bool IsFirstInstance
        {
            get { return _isfirst; }
        }

        public void Close()
        {
            _mutex.ReleaseMutex();
            _isRunning = false;
            Write(EXIT_STRING);
            Thread.Sleep(3); // give time for thread shutdown
        }
        public void SubmitParameters()
        {
            var pars = Environment.GetCommandLineArgs();
            StringBuilder sb = new StringBuilder();
            for (int i=1; i<pars.Length; i++)
            {
                sb.AppendFormat("{0}\n", pars[i]);
            }
            Write(sb.ToString());
        }
    }
}
