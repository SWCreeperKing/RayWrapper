using System.Collections.Concurrent;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.GameConsole;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;
using static RayWrapper.Vars.Logger;

namespace RayWrapper.Collision;

public static class Collision
{
    // performance stats
    public static long[] CollisionTime { get; } = new long[100];
    public static double TimeAverage { get; private set; }
    public static long CollisionHigh { get; private set; }
    public static long CurrentCollision { get; private set; }
    public static int TimeKeeper { get; private set; }

    public static ConcurrentDictionary<string, string> collisionRules = new();
    public static int collisionTimerMs = 2;
    public static bool isPaused;

    private static ConcurrentDictionary<long, Collider> _collisionObjects = new();
    private static ConcurrentBag<Collider> _collisionObjectsRemove = new();
    private static ConcurrentBag<Collider> _collisionObjectsAdd = new();
    private static List<Collider>[] _collisionTable = null!;
    private static List<string>[] _collisionTagTable = null!;
    private static Rectangle[] _collisionSectors = null!;
    private static long _lastPhysicTick;
    private static bool _runPhysics = true;

    public static void InitPhysics(int sectorsX = 4, int sectorsY = 3)
    {
        Log(Level.Info, "RayWrapper.Collision: Init");
        collisionRules.TryAdd("def", "def");
        var sectorSize = WindowSize / new Vector2(sectorsX, sectorsY);
        _collisionSectors = new Rectangle[sectorsX * sectorsY];
        _collisionTable = new List<Collider>[sectorsX * sectorsY];
        _collisionTagTable = new List<string>[_collisionTable.Length];
        for (var i = 0; i < _collisionTable.Length; i++)
        {
            _collisionTable[i] = new List<Collider>();
            _collisionTagTable[i] = new List<string>();
        }

        for (var y = 0; y < sectorsY; y++)
        for (var x = 0; x < sectorsX; x++)
        {
            _collisionSectors[sectorsX * y + x] = AssembleRectFromVec(sectorSize * new Vector2(x, y), sectorSize);
        }

        var ts = new CancellationTokenSource();
        var ct = ts.Token;

        StaticRender += () =>
        {
            foreach (var c in _collisionObjects)
            {
                try
                {
                    c.Value.Render();
                }
                catch (KeyNotFoundException)
                {
                    // ignore the list modification error
                }
            }
        };

        StaticDispose += () => ts.Cancel();

        Task.Run(async () =>
        {
            _lastPhysicTick = GetTimeMs();
            try
            {
                while (_runPhysics)
                {
                    if (isPaused) await Task.Delay(25);
                    var startTime = GetTimeMs();

                    foreach (var obj in _collisionObjectsAdd)
                    {
                        _collisionObjects.TryAdd(obj.currentId, obj);
                    }

                    await PhysicUpdate();
                    await CollisionDetect();
                    await PostCollision();

                    foreach (var obj in _collisionObjectsRemove)
                    {
                        _collisionObjects.Remove(obj.currentId, out _);
                    }
                    
                    _collisionObjectsAdd.Clear();
                    _collisionObjectsRemove.Clear();

                    await Task.Run(() => AddTime(GetTimeMs() - startTime));
                    await Task.Delay(collisionTimerMs);

                    if (!ct.IsCancellationRequested) continue;
                    Log(Level.Special, "Closed PhysicThread");
                    return;
                }
            }
            catch (Exception e)
            {
                Log(Level.Error, $"RayWrapper.Collision: {e}");
            }
        }, ct);

        CommandRegister.RegisterCommand<CollisionCommands>();
    }

    public static async Task PhysicUpdate()
    {
        var delta = GetTimeMs() - _lastPhysicTick;
        _lastPhysicTick = GetTimeMs();
        foreach (var c in _collisionObjects) c.Value.PhysicUpdate(delta);
    }

    public static async Task CollisionDetect()
    {
        for (var i = 0; i < _collisionSectors.Length; i++)
        {
            _collisionTable[i].Clear();
            _collisionTagTable[i].Clear();
            foreach (var c in _collisionObjects)
            {
                // to reduce mem allow, as `Where` will cause like 100mb of extra ram xd
                var collider = c.Value;
                if (!collider.SampleCollision(_collisionSectors[i])) continue;
                _collisionTable[i].Add(collider);
                if (!_collisionTagTable[i].Contains(collider.tag)) _collisionTagTable[i].Add(collider.tag);
            }
        }

        for (var i = 0; i < _collisionTable.Length; i++)
        {
            var list = _collisionTable[i];
            if (list.Count < 2) continue;

            var tags = _collisionTagTable[i];
            var dontCheck = true;
            for (var j = 0; j < tags.Count && dontCheck; j++)
            for (var k = 0; k < tags.Count && dontCheck; k++)
            {
                if (collisionRules.ContainsKey(tags[j]) && collisionRules[tags[j]] == tags[k]) dontCheck = false;
            }

            if (dontCheck) continue;
            await CheckCollision(list);
        }
    }

    public static async Task CheckCollision(List<Collider> objs)
    {
        if (objs.Count < 2) return;
        for (var i = 0; i < objs.Count - 1; i++)
        for (var j = i + 1; j < objs.Count; j++)
        {
            objs[i].DoCollision(objs[j]);
        }
    }

    public static async Task PostCollision()
    {
        foreach (var c in _collisionObjects) c.Value.PostCollision();
    }

    public static void AddTime(long ms)
    {
        CurrentCollision = CollisionTime[TimeKeeper++] = ms;
        TimeKeeper %= CollisionTime.Length;
        CollisionHigh = Math.Max(CollisionHigh, ms);
        TimeAverage = CollisionTime.Sum() / (double) CollisionTime.Length;
    }

    public static void AddObject(Collider c) => _collisionObjectsAdd.Add(c);
    public static void RemoveObject(Collider c) => _collisionObjectsRemove.Add(c);
    public static long CountColliders() => _collisionObjects.Count;
}

public class CollisionCommands : ICommandModule
{
    [Command("count"), Help("Counts the amount of colliders")]
    public static string Count(string[] args) => $"There are [{Collision.CountColliders()}] colliders";
}