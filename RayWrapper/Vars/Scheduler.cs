using System;

namespace RayWrapper.Vars
{
    public class Scheduler
    {
        public long incrementMs;
        public Action onTime;

        private long _nextTime;

        public Scheduler(long incrementMs, Action onTime, bool setTime = true)
        {
            (this.incrementMs, this.onTime) = (incrementMs, onTime);
            if (setTime) SetTime(GameBox.GetTimeMs());
            else _nextTime = GameBox.GetTimeMs();
        }

        public void SetTime(long time) => _nextTime = time + incrementMs;

        public void TestTime(long time)
        {
            if (time < _nextTime) return;
            onTime.Invoke();
            SetTime(time);
        }
    }
}