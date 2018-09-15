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
