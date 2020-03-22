using System.Collections;
using System.Collections.Generic;

namespace TCPlayer.Engine
{
    public struct EqualizerConfig: IEnumerable<double>
    {
        public double Bass
        {
            get;
            set;
        }

        public double Mid
        {
            get;
            set;
        }

        public double Treble
        {
            get;
            set;
        }

        public IEnumerator<double> GetEnumerator()
        {
            yield return Bass;
            yield return Mid;
            yield return Treble;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return Bass;
            yield return Mid;
            yield return Treble;
        }
    }
}
