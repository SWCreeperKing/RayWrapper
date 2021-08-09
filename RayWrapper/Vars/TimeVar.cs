using System;
using System.Text;

namespace RayWrapper.Vars
{
    public class TimeVar
    {
        private static readonly short[] _convertTimes = {1000, 60, 60, 24, 7};
        private static readonly string[] _timeChars = {"ms", "s", "m", "h", "d", "w"};

        public bool writeZeros = false;
        public long[] times = {0, 0, 0, 0, 0, 0}; //ms, s, m, h, d, w

        public TimeVar(long ms)
        {
            times[0] = ms;
            Update();
        }

        public void Update()
        {
            for (var i = 0; i < times.Length - 1; i++)
            {
                times[i + 1] += times[i] / _convertTimes[i];
                times[i] %= _convertTimes[i];
            }
        }

        /// <param name="index">ms = 0, s = 1, m = 2, h = 3, d = 4, w = 5</param>
        public void AddTime(int index, long amnt)
        {
            times[index] += amnt;
            Update();
        }

        /// <param name="index">ms = 0, s = 1, m = 2, h = 3, d = 4, w = 5</param>
        public double ToTime(int index)
        {
            var times = new double[this.times.Length];

            for (var i = 0; i < index; i++) times[i + 1] += (this.times[i] + times[i]) / _convertTimes[i];

            for (var i = this.times.Length - 1; i > index; i--)
                times[i - 1] += (this.times[i] + times[i]) * _convertTimes[i - 1];

            return times[index] + this.times[index];
        }

        public override string ToString() => ToString(0);

        public string ToString(int start, int end = -1)
        {
            if (end == -1) end = times.Length - 1;
            StringBuilder sb = new();
            for (var i = end; i >= start; i--)
                if (times[i] != 0 || writeZeros)
                {
                    sb.Append(times[i]).Append(_timeChars[i]);
                    if (i != 0) sb.Append(' ');
                }

            return sb.ToString();
        }

        public static implicit operator TimeVar(long ms) => new(ms);
        public static implicit operator long(TimeVar t) => (long) t.ToTime(0);

        public static TimeVar operator +(TimeVar t1, TimeVar t2) => (long) (t1.ToTime(0) + t2.ToTime(0));
        public static TimeVar operator -(TimeVar t1, TimeVar t2) => (long) (t1.ToTime(0) - t2.ToTime(0));
    }
}