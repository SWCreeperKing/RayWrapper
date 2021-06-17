using System;

namespace RayWrapper.Vars
{
    public class Scheduler
    {
        public long incrementMs;
        public Action onTime;
        public bool compensate;
        
        private long _nextTime;
        private long _compensation;

        public Scheduler(long incrementMs, Action onTime)
        {
            this.incrementMs = incrementMs;
            this.onTime = onTime;
        }

        public void SetTime(long time) => _nextTime = time + incrementMs;
        
        public void TestTime(long time)
        {
            if (time < _nextTime) return;
            _compensation += time - _nextTime;
            onTime.Invoke();
            SetTime(time);
            if (compensate) Compensate();
        }

        public void Compensate()
        {
            while (_compensation >= incrementMs)
            {
                onTime.Invoke();
                _compensation -= incrementMs;
            }
        }
    }
}