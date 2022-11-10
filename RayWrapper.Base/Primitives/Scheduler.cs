using RayWrapper.Base.GameBox;
using static RayWrapper.Base.GameBox.AttributeManager;
using static RayWrapper.Base.GameBox.GameBox;
using static RayWrapper.Base.GameBox.Logger.Level;

namespace RayWrapper.Base.Primitives;

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
        if (setTime) SetTime(GetTimeMs());
        else _nextTime = GetTimeMs();
    }

    public void SetTime(long time) => _nextTime = time + incrementMs;

    public void TestTime(long time)
    {
        if (time < _nextTime) return;
        onTime();
        SetTime(time);
    }
}

public static class SchedulerSetup
{
    public static bool conserveCpu;

    private static bool _isEnd;
    private static Task _schedulerTask;
    private static List<Scheduler> _schedulers = new();
    private static List<Scheduler> _schedulerQueue = new();
    
    [GameBoxWedge(PlacerType.AfterInit)]
    public static void InitScheduler()
    {
        _schedulerTask = Task.Run(RunSchedulers);
    }

    [GameBoxWedge(PlacerType.BeforeDispose)]
    public static void EndSchedulers()
    {
        _isEnd = true;
        Logger.Log(Other, "Waiting for schedulers to end");
        while (!_schedulerTask.IsCompleted) Task.Delay(10).GetAwaiter().GetResult();
        Logger.Log(Special, "All Tasks ended successfully");
    }

    private static async Task RunSchedulers()
    {
        while (!_isEnd)
        {
            try
            {
                foreach (var scheduler in _schedulers) scheduler.TestTime(GetTimeMs());
                _schedulers.AddRange(_schedulerQueue);
                _schedulerQueue.RemoveAll(s => _schedulers.Contains(s));
            }
            catch (Exception e)
            {
                Logger.Log(Error, e.ToString());
            }

            if (conserveCpu) await Task.Delay(10);
        }
    }
    
    public static void AddScheduler(Scheduler schedule) => _schedulerQueue.Add(schedule);
}