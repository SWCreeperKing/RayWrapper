using System;

namespace RayWrapper.Vars
{
    public class Scheduler
    {
        public long incrementMs
        {
            get => _incMs * 10;
            set => _incMs = Math.Max(10, value) / 10;
        }
        public Action onTime;

        private long _nextTime;
        private long _incMs;

        // increment time's min is at 10ms
        public Scheduler(long incrementMs, Action onTime, bool setTime = true)
        {
            (this.incrementMs, this.onTime) = (incrementMs, onTime);
            if (setTime) SetTime(GameBox.GetTimeMs());
            else _nextTime = GameBox.GetTimeMs();
        }

        public void SetTime(long time) => _nextTime = time + _incMs;

        public void TestTime(long time)
        {
            if (time < _nextTime) return;
            onTime.Invoke();
            SetTime(_nextTime);
        }
    }
}