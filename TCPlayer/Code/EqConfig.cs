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

namespace TCPlayer.Code
{
    public class EqConfig
    {
        private readonly double[] _values;

        public EqConfig()
        {
            _values = new double[3];
        }

        public double Bass
        {
            get { return _values[0]; }
            set { _values[0] = value; }
        }

        public double Mid
        {
            get { return _values[1]; }
            set { _values[1] = value; }
        }

        public double Treble
        {
            get { return _values[2]; }
            set { _values[2] = value; }
        }

        public float this[int i]
        {
            get
            {
                return (float)_values[i];
            }
        }
    }
}
