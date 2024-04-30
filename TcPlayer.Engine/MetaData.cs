namespace TcPlayer.Engine;

public sealed class MetaData : IEquatable<MetaData>
{
    public IList<string> Data { get; init; }
    
    public byte[] Cover { get; init; }

    public string CoverMime { get; init; }

    public MetaData()
    {
        CoverMime = string.Empty;
        Cover = Array.Empty<byte>();
        Data = new List<string>();
    }


    public bool Equals(MetaData? other)
    {
        if (Cover.Length != other?.Cover.Length)
            return false;

        if (CoverMime != other?.CoverMime)
            return false;

        if (other?.Data.Count != Data.Count)
            return false;

        for (int i=0; i< Data.Count; i++)
        {
            if (Data[i] != other.Data[i])
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
        foreach (var item in Data)
        {
            hash.Add(item);
        }
        
        return hash.ToHashCode();
    }
}
