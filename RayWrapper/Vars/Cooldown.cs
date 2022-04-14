using System;
using static RayWrapper.GameBox;

namespace RayWrapper.Vars;

/// <summary>
/// this is an object that keeps track of cooldowns. see: <see cref="UpdateTime"/> for a better example of what this is used for 
/// </summary>
public class Cooldown
{
    public long durationMs;
    public long lastTick;

    public Cooldown(long durationMs, bool startWithCooldown = false) =>
        (this.durationMs, lastTick) = (durationMs, GetTimeMs() - (startWithCooldown ? 0 : durationMs));

    /// <summary>
    /// update the cooldown and return if it's ready
    /// </summary>
    /// <param name="resetTimeOnTrue">if to call <see cref="ResetTick"/> the moment this returns true (true by default)</param>
    /// <returns>if the cooldown is ready (resets cooldown if <paramref name="resetTimeOnTrue"/> is true)</returns>
    public bool UpdateTime(bool resetTimeOnTrue = true)
    {
        if (lastTick + durationMs > GetTimeMs()) return false;
        if (resetTimeOnTrue) ResetTick();
        return true;
    }

    /// <summary>
    /// Resets cooldown timer
    /// </summary>
    public void ResetTick() => lastTick = GetTimeMs();

    public float GetRemainingTime() => Math.Max(durationMs - (GetTimeMs() - lastTick), 0);
    public float GetTimePercent() => Math.Clamp((float) (GetTimeMs() - lastTick) / durationMs, 0, 1);
}