using System;
using static RayWrapper.GameBox;

namespace RayWrapper.Vars
{
    public class Cooldown
    {
        public long durationMs;
        public long lastTick;

        public Cooldown(long durationMs, bool startWithCooldown = false) =>
            (this.durationMs, lastTick) = (durationMs, GetTimeMs() - (startWithCooldown ? 0 : durationMs));

        public bool UpdateTime(bool resetTimeOnTrue = true)
        {
            if (lastTick + durationMs > GetTimeMs()) return false;
            if (resetTimeOnTrue) ResetTick();
            return true;
        }

        public void ResetTick() => lastTick = GetTimeMs();
        public float GetRemainingTime() => Math.Max(durationMs - (GetTimeMs() - lastTick), 0);
        public float GetTimePercent() => Math.Clamp((float)(GetTimeMs() - lastTick) / durationMs, 0, 1);
    }
}