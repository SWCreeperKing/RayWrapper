using System;
using static RayWrapper.GameBox;

namespace RayWrapper.Vars
{
    public class Cooldown
    {
        public long durationMs;
        public long LastTick { get; private set; }

        public Cooldown(long durationMs, bool startWithCooldown = false) =>
            (this.durationMs, LastTick) = (durationMs, GetTimeMs() - (startWithCooldown ? 0 : durationMs));

        public bool UpdateTime()
        {
            var time = GetTimeMs();
            if (LastTick + durationMs < time) return false;
            LastTick = time;
            return true;
        }

        public void ResetRick() => LastTick = GetTimeMs();
        public float GetRemainingTime() => Math.Max(durationMs - GetTimeMs() - LastTick, 0);
        public float GetTimePercent() => (float)(GetTimeMs() - LastTick) / durationMs;
    }
}