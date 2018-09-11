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
using System.Collections.Generic;

namespace TCPlayer.Controls.Network
{
    internal class Constants
    {
        public static IEnumerable<NetworkSearchProvider> NetworkProviders
        {
            get
            {
                yield return new NetworkSearchProvider
                {
                    Name = "Spotify",
                    UriTemplate = "https://open.spotify.com/search/results/{0}"
                };
                yield return new NetworkSearchProvider
                {
                    Name = "Soundcloud",
                    UriTemplate = "https://soundcloud.com/search?q={0}"
                };
                yield return new NetworkSearchProvider
                {
                    Name = "Youtube",
                    UriTemplate = "https://www.youtube.com/results?search_query={0}"
                };
                yield return new NetworkSearchProvider
                {
                    Name = "Discogs",
                    UriTemplate = "https://www.discogs.com/search/?q={0}&type=all"
                };
                yield return new NetworkSearchProvider
                {
                    Name = "Google",
                    UriTemplate = "https://www.google.com/search?q={0}"
                };
            }
        }
    }
}
