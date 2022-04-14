using System;

namespace RayWrapper.Vars;

/// <summary>
/// Schedulers are for timing events,
/// when using a low ms timer, you should heed with caution.
/// it is recommended to go above 50ms.
///
/// to add a scheduler use <see cref="GameBox.AddScheduler(Scheduler)"/>
/// </summary>
public class Scheduler
{
    public long incrementMs;
    public Action onTime;

    private long _nextTime;

    /// <summary>
    /// Create a new <see cref="Scheduler"/>, use <see cref="GameBox.AddScheduler(Scheduler)"/> to add it to the <see cref="GameBox"/>
    /// </summary>
    /// <param name="incrementMs">how long in MS should this increment (over 50ms is recommended)</param>
    /// <param name="onTime">an action for what happens every time an increment happens</param>
    /// <param name="setTime">if it should not execute <paramref name="onTime"/> right away</param>
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