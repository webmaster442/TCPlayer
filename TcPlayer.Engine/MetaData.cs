namespace TcPlayer.Engine;

public sealed class MetaData : IEquatable<MetaData>
{
    public const string KeyArtist = "artist";
    public const string KeyTitle = "title";
    public const string KeyAlbum = "album";
    public const string KeyYear = "year";


    public IDictionary<string, string> DataDictionary { get; }

    public IReadOnlyList<string> Data => DataDictionary.Values.ToList();
    
    public byte[] Cover { get; init; }

    public string CoverMime { get; init; }

    public MetaData()
    {
        CoverMime = string.Empty;
        Cover = [];
        DataDictionary = new Dictionary<string, string>();
    }

    public bool Equals(MetaData? other)
    {
        if (other == null)
            return false;

        if (Cover.Length != other.Cover.Length)
            return false;

        if (CoverMime != other.CoverMime)
            return false;

        if (DataDictionary.Count != other.DataDictionary.Count)
            return false;

        foreach (var item in DataDictionary)
        {
            if (!other.DataDictionary.TryGetValue(item.Key, out string? value1)
                || !DataDictionary.TryGetValue(item.Key, out string? value2)
                || value1 != value2)
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as MetaData);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Cover.Length);
        hash.Add(CoverMime);
        foreach (var item in DataDictionary)
        {
            hash.Add(item.Key);
            hash.Add(item.Value);
        }
        
        return hash.ToHashCode();
    }
}
