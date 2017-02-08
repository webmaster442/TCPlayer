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
using Mp4Chapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TCPlayer.Code
{
    internal class ChapterProvider
    {
        private Dictionary<double, string> _data;

        private const double Minute = 60.0d;

        private ContextMenu _target;

        public event EventHandler<double> ChapterClicked;

        public ChapterProvider(ContextMenu target)
        {
            _data = new Dictionary<double, string>();
            _target = target;
        }

        private void CreateChapters(double lenght)
        {
            var divider = Minute;
            if (lenght < 10 * Minute) divider = Minute;
            else if (lenght >= 10 * Minute) divider = 5.0 * Minute;

            double position = 0;
            _data.Add(0, "Jump to begining");
            while (position + divider < lenght)
            {
                position += divider;
                var str = string.Format("Jump to {0}", TimeSpan.FromSeconds(position));
                _data.Add(position, str);
            }
        }

        public void CreateChapters(string filename, double parsedlength)
        {
            try
            {
                _data.Clear();

                var extension = Path.GetExtension(filename).ToLower();

                if ((extension != ".mp4") && (extension != ".m4a") &&(extension != ".m4b"))
                {
                    CreateChapters(parsedlength);
                    return;
                }

                using (var stream = File.OpenRead(filename))
                {
                    var extractor = new ChapterExtractor(new StreamWrapper(stream));
                    extractor.Run();
                    foreach (var c in extractor.Chapters)
                    {
                        _data.Add(c.Time.TotalSeconds, c.Name);
                    }
                }
            }
            catch (Exception)
            {
                _data.Clear();
                CreateChapters(parsedlength);
            }
            DrawToMenu();
        }

        private void DecomissionMenu()
        {
            foreach (MenuItem child in _target.Items)
            {
                child.Click -= Mnu_Click;
            }
        }

        private void DrawToMenu()
        {
            DecomissionMenu();
            _target.Items.Clear();
            foreach (var chapter in _data)
            {
                MenuItem mnu = new MenuItem();
                mnu.Header = chapter.Value;
                mnu.Tag = chapter.Key;
                mnu.Style = (System.Windows.Style)Application.Current.MainWindow.FindResource("SubMenuItem");
                mnu.Icon = Application.Current.MainWindow.FindResource("IconArrowRight");
                mnu.Click += Mnu_Click;
                _target.Items.Add(mnu);
            }
        }

        private void Mnu_Click(object sender, RoutedEventArgs e)
        {
            var pos = Convert.ToDouble(((MenuItem)sender).Tag);
            ChapterClicked?.Invoke(sender, pos);
        }
    }
}
