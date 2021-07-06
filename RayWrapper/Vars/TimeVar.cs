using System.Text;

namespace RayWrapper.Vars
{
    public class TimeVar
    {
        private static readonly short[] _convertTimes = {1000, 60, 60, 24, 7};
        private static readonly string[] _timeChars = {"ms", "s", "m", "h", "d", "w"};

        public bool writeZeros = false;
        public long[] _times = {0, 0, 0, 0, 0, 0}; //ms, s, m, h, d, w

        public TimeVar(long ms)
        {
            _times[0] = ms;
            Update();
        }

        public void Update()
        {
            for (var i = 0; i < _times.Length - 1; i++)
            {
                _times[i + 1] += _times[i] / _convertTimes[i];
                _times[i] %= _convertTimes[i];
            }
        }

        /// <param name="index">ms = 0, s = 1, m = 2, h = 3, d = 4, w = 5</param>
        public void AddTime(int index, long amnt)
        {
            _times[index] += amnt;
            Update();
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            for (var i = _times.Length - 1; i >= 0; i--)
                if (_times[i] > 0 || writeZeros)
                {
                    sb.Append(_times[i]).Append(_timeChars[i]);
                    if (i != 0) sb.Append(' ');
                }
            
            return sb.ToString();
        }
    }
}