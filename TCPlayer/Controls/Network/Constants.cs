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
