using System.Text;

namespace RayWrapper.Vars;

public class TimeVar
{
    private static readonly short[] _convertTimes = { 1000, 60, 60, 24, 7 };
    private static readonly string[] _timeChars = { "ms", "s", "m", "h", "d", "w" };

    public bool writeZeroes = false;
    public long[] times; //ms, s, m, h, d, w

    /// <param name="index">ms = 0, s = 1, m = 2, h = 3, d = 4, w = 5</param>
    public TimeVar(long amt, int index = 0)
    {
        times = new long[] { 0, 0, 0, 0, 0, 0 };
        index = Math.Clamp(index, 0, times.Length);
        times[index] = amt;
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
    public void AddTime(long amount, int index = 0)
    {
        index = Math.Clamp(index, 0, times.Length - 1);
        times[index] += amount;
        Update();
    }

    /// <param name="index">ms = 0, s = 1, m = 2, h = 3, d = 4, w = 5</param>
    public double ToTime(int index)
    {
        index = Math.Clamp(index, 0, this.times.Length - 1);
        var times = new double[this.times.Length];

        for (var i = 0; i < index; i++) times[i + 1] += (this.times[i] + times[i]) / _convertTimes[i];

        for (var i = this.times.Length - 1; i > index; i--)
        {
            times[i - 1] += (this.times[i] + times[i]) * _convertTimes[i - 1];
        }

        return times[index] + this.times[index];
    }

    public override string ToString() => ToString(0);

    public string ToString(int start, int end = -1)
    {
        StringBuilder sb = new();
        start = Math.Clamp(start, 0, times.Length - 1);
        end = Math.Clamp(end, -1, times.Length - 1);
        if (end == -1 || end < start) end = times.Length - 1;
        sb.Clear();

        for (var i = end; i >= start; i--)
        {
            if (times[i] == 0 && !writeZeroes) continue;
                
            sb.Append(times[i]).Append(_timeChars[i]);
            if (i != 0) sb.Append(' ');
        }

        return sb.ToString();
    }

    public static implicit operator TimeVar(long ms) => new(ms);
    public static implicit operator long(TimeVar t) => (long)t.ToTime(0);

    public static TimeVar operator +(TimeVar t1, TimeVar t2) => (long)(t1.ToTime(0) + t2.ToTime(0));
    public static TimeVar operator -(TimeVar t1, TimeVar t2) => (long)(t1.ToTime(0) - t2.ToTime(0));
}