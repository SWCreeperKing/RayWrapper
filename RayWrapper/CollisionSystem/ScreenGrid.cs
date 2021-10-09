using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Raylib_cs;
using static RayWrapper.GameBox;

namespace RayWrapper.CollisionSystem
{
    public class ScreenGrid
    {
        public Dictionary<(int x, int y), List<int>> collisionGrid = new();

        private List<Collider> subCol = new();
        private long lastTick = -1;
        private const float screenPercent = .1f;
        private List<Collider> removeQueue = new();
        private List<Collider> addQueue = new();
        private Dictionary<(int x, int y), Rectangle> rects = new();
        private List<(int x, int y)> keys = new();

        public ScreenGrid()
        {
            var incrX = (int)Math.Ceiling(WindowSize.X * screenPercent);
            var incrY = (int)Math.Ceiling(WindowSize.Y * screenPercent);
            for (var x = 0; x < WindowSize.X; x += incrX)
            for (var y = 0; y < WindowSize.Y; y += incrY)
                rects.Add((x, y), new Rectangle(x, y, incrX, incrY));
            foreach (var k in rects.Keys) collisionGrid.Add(k, new List<int>());
            keys.AddRange(rects.Keys);
        }

        public async Task Update()
        {
            var thisTick = GetTimeMs();
            if (lastTick == -1) lastTick = thisTick;
            var deltaTime = thisTick - lastTick;
            foreach (var k in collisionGrid.Keys) collisionGrid[k].Clear();

            await Task.Run(() => Sort(deltaTime));
            await Task.Run(CollDetect);

            if (addQueue.Any())
            {
                foreach (var c in addQueue) subCol.Add(c);
                addQueue.Clear();
            }

            if (removeQueue.Any())
            {
                foreach (var c in removeQueue) subCol.Remove(c);
                removeQueue.Clear();
            }

            lastTick = thisTick;
        }

        public void Draw(bool debug)
        {
            for (var i = 0; i < subCol.Count; i++) subCol[i].Render();
            if (!debug || !isCollisionSystem) return;
            foreach (var rect in rects.Values) rect.DrawHallowRect(Color.GREEN);
        }

        public void SubscribeCollider(Collider c) => addQueue.Add(c);
        public void UnSubscribeCollider(Collider c) => removeQueue.Add(c);

        public void Sort(float deltaTime)
        {
            for (var i = 0; i < subCol.Count; i++)
            {
                for (var j = 0; j < keys.Count; j++)
                    if (subCol[i].SampleCollision(rects[keys[j]]))
                        collisionGrid[keys[j]].Add(i);
                if (subCol[i].velocity != Vector2.Zero) subCol[i].Position += subCol[i].velocity * deltaTime;
                subCol[i].Update();
            }
        }

        public void CollDetect()
        {
            var lists = collisionGrid.Values.Where(l => l.Count > 1).ToList();
            if (lists.Any())
                for (var l = 0; l < lists.Count; l++) // second slow down if ~5k s: 25ms 
                {
                    var list = lists[l];
                    if (!list.Any()) continue;
                    for (var i = 0; i < list.Count - 1; i++)
                    for (var j = i + 1; j < list.Count; j++)
                        subCol[list[i]].DoCollision(subCol[list[j]]);
                }
        }
    }
}