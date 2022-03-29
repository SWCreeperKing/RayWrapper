using System.Numerics;
using Raylib_CsLo;
using RayWrapperTester;
using static RayWrapper.GameBox;

namespace RayWrapper.Collision;

public static class Starter
{
    // performance stats
    public static long[] CollisionTime { get; } = new long[100];
    public static double TimeAverage { get; private set; }
    public static long CollisionHigh { get; private set; }
    public static long CurrentCollision { get; private set; }
    public static int TimeKeeper { get; private set; }

    public static int CollisionTimerMs = 1;
    public static Dictionary<string, List<PhysicObject>> CollisionObjects = new();

    private static readonly Dictionary<string, List<string>> CollisionTags = new();

    private static Task _collisionThread;

    public static void InitPhysics()
    {
        Physac.InitPhysics();
        _collisionThread = new Task(async () =>
        {
            var startTime = GetTimeMs();
            List<(string t1, string t2)> collisionCache = new();

            foreach (var (t1, t2) in CollisionTags)
            {
                CollisionObjects[t1].ForEach(arr =>
                {
                    //todo: collision
                    foreach (var tag in t2)
                    {
                        // if (collisionCache.Contains((, )))
                    }
                });

                // Raylib.CheckCollisionRecs();
                // Raylib.CheckCollisionCircles();
                // Raylib.CheckCollisionCircleRec();
                // Raylib.CheckCollisionCircleRec();
            }

            await Task.Run(() => AddTime(GetTimeMs() - startTime));
            await Task.Delay(CollisionTimerMs);
        });
    }

    public static void DisposePhysics()
    {
        _collisionThread.Dispose();
        Physac.ClosePhysics();
    }

    public static void AddTime(long ms)
    {
        CurrentCollision = CollisionTime[TimeKeeper++] = ms;
        TimeKeeper %= CollisionTime.Length;
        CollisionHigh = Math.Max(CollisionHigh, ms);
        TimeAverage = CollisionTime.Sum() / (double) CollisionTime.Length;
    }

    public static void AddCollisionRule(string tag1, string tag2)
    {
        MakeCollisionRule(tag1, tag2);
        MakeCollisionRule(tag2, tag1);
    }

    private static void MakeCollisionRule(string tag1, string tag2)
    {
        if (CollisionTags.ContainsKey(tag1))
        {
            if (CollisionTags[tag1].Contains(tag2)) return;
            CollisionTags[tag1].Add(tag2);
        }
        else CollisionTags.Add(tag1, new List<string> { tag2 });
    }

    public static Vector2 GetVertex(this PhysicsBodyData data, int vertexIndex)
    {
        unsafe
        {
            return Physac.GetPhysicsShapeVertex(&data, vertexIndex);
        }
    }
}