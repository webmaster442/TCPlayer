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
using System.Web;

namespace TCPlayer.Controls.Network
{
    public class NetworkSearchProvider : IEquatable<NetworkSearchProvider>
    {
        public string Name { get; set; }
        public string UriTemplate { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as NetworkSearchProvider);
        }

        public bool Equals(NetworkSearchProvider other)
        {
            return other != null &&
                   Name == other.Name &&
                   UriTemplate == other.UriTemplate;
        }

        public string GetFullUri(string parameter)
        {
            if (string.IsNullOrEmpty(UriTemplate)) return null;
            return string.Format(UriTemplate, HttpUtility.UrlEncode(parameter));
        }

        public override int GetHashCode()
        {
            var hashCode = -2080565799;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UriTemplate);
            return hashCode;
        }

        public static bool operator ==(NetworkSearchProvider provider1, NetworkSearchProvider provider2)
        {
            return EqualityComparer<NetworkSearchProvider>.Default.Equals(provider1, provider2);
        }

        public static bool operator !=(NetworkSearchProvider provider1, NetworkSearchProvider provider2)
        {
            return !(provider1 == provider2);
        }
    }
}
