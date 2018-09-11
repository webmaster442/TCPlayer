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
