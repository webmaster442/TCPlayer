using System.Collections;

namespace TcPlayer.Engine
{
    public class MetaData : IEquatable<MetaData>, IEnumerable<string>
    {
        private readonly List<string> _data;

        public MetaData()
        {
            _data = new List<string>();
        }

        public MetaData Add(params string[] data)
        {
            _data.AddRange(data);
            return this;
        }

        public bool Equals(MetaData? other)
        {
            if (other?._data.Count != _data.Count)
                return false;

            for (int i=0; i<_data.Count; i++)
            {
                if (_data[i] != other._data[i])
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
            foreach (var item in _data) 
            {
                hash.Add(item);
            }
            return hash.ToHashCode();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}
