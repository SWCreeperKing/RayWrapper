using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Physac;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

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
    public static List<PhysicObject> CollisionObjects = new();

    private static List<PhysicObject>[] _collisionTable;
    private static Rectangle[] _collisionSectors;
    private static Task _collisionThread;

    public static void InitPhysics(int sectorsX = 4, int sectorsY = 3)
    {
        Physac.InitPhysics();

        var sectorSize = WindowSize / new Vector2(sectorsX, sectorsY);
        _collisionSectors = new Rectangle[sectorsX * sectorsY];
        _collisionTable = new List<PhysicObject>[sectorsX * sectorsY];
        for (var y = 0; y < sectorsY; y++)
        for (var x = 0; x < sectorsX; x++)
        {
            _collisionSectors[sectorsX * y + x] = AssembleRectFromVec(sectorSize * new Vector2(x, y), sectorSize);
        }

        _collisionThread = Task.Run(async () =>
        {
            while (true)
            {
                UpdatePhysics();
                var startTime = GetTimeMs();

                for (var i = 0; i < _collisionSectors.Length; i++)
                {
                    _collisionTable[i] = CollisionObjects.Where(obj => obj.HasCollision(_collisionSectors[i])).ToList();
                }

                foreach (var objs in _collisionTable.Where(arr => arr.Count > 1))
                {
                    await CheckCollision(objs);
                }

                await Task.Run(() => AddTime(GetTimeMs() - startTime));
                await Task.Delay(CollisionTimerMs);
            }
        });
        
        StaticRender += () =>
        {
            unsafe
            {
                var bodiesCount = GetPhysicsBodiesCount();
                for (var i = 0; i < bodiesCount; i++)
                {
                    var body = GetPhysicsBody(i);

                    if (body == (int*) 0) continue;
                    var vertexCount = GetPhysicsShapeVerticesCount(i);
                    for (var j = 0; j < vertexCount; j++)
                    {
                        // Get physics bodies shape vertices to draw lines
                        // Note: GetPhysicsShapeVertex() already calculates rotation transformations
                        var vertexA = GetPhysicsShapeVertex(body, j);

                        var jj = j + 1 < vertexCount
                            ? j + 1
                            : 0; // Get next vertex or first to close the shape
                        var vertexB = GetPhysicsShapeVertex(body, jj);

                        DrawLineV(vertexA, vertexB, GREEN); // Draw a line between two vertex positions
                    }
                }
            }
        };
    }

    public static async Task CheckCollision(List<PhysicObject> objs)
    {
        for (var i = 0; i < objs.Count; i++)
        for (var j = i + 1; i < objs.Count; j++)
        {
            if (objs[i].HasCollision(objs[j])) objs[i].ExecuteCollision(objs[j]);
        }
    }

    public static void DisposePhysics()
    {
        _collisionThread.Dispose();
        ClosePhysics();
    }

    public static void AddTime(long ms)
    {
        CurrentCollision = CollisionTime[TimeKeeper++] = ms;
        TimeKeeper %= CollisionTime.Length;
        CollisionHigh = Math.Max(CollisionHigh, ms);
        TimeAverage = CollisionTime.Sum() / (double) CollisionTime.Length;
    }

    public static Vector2 GetVertex(this PhysicsBodyData data, int vertexIndex)
    {
        unsafe
        {
            return GetPhysicsShapeVertex(&data, vertexIndex);
        }
    }
}