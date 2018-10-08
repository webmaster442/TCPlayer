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
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Media.Imaging;

namespace TCPlayer.Code.iTunesLookup
{
    internal static class iTunesLookup
    {
        public static Task<byte[]> GetCoverFor(string query)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        IWebProxy defaultProxy = WebRequest.DefaultWebProxy;
                        if (defaultProxy != null)
                        {
                            defaultProxy.Credentials = CredentialCache.DefaultCredentials;
                            client.Proxy = defaultProxy;
                        }

                        var encoded = HttpUtility.UrlEncode(query);

                        var fulladdress = $"https://itunes.apple.com/search?term={encoded}&media=music&limit=1";

                        var response = client.DownloadString(fulladdress);

                        var jsonSerializer = new JavaScriptSerializer();

                        var responseObject = jsonSerializer.Deserialize<Rootobject>(response);

                        string artwork = responseObject.results[0].artworkUrl100;
                        artwork = artwork.Replace("100x100", "600x600");

                        return client.DownloadData(artwork);
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        public static BitmapImage CreateBitmap(byte[] artworkData)
        {
            using (var ms = new MemoryStream(artworkData))
            {
                BitmapImage ret = new BitmapImage();
                ret.BeginInit();
                ret.CacheOption = BitmapCacheOption.OnLoad;
                ret.StreamSource = ms;
                ret.EndInit();
                if (ret.CanFreeze)
                    ret.Freeze();
                return ret;
            }
        }
    }
}
