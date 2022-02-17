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

        private IList<Collider> subCol = new List<Collider>();
        private long lastTick = -1;
        private const float screenPercent = .1f;
        private IList<Collider> removeQueue = new List<Collider>();
        private IList<Collider> addQueue = new List<Collider>();
        private IDictionary<(int x, int y), Rectangle> rects = new Dictionary<(int x, int y), Rectangle>();
        private List<(int x, int y)> keys = new();

        public ScreenGrid()
        {
            var incrX = (int) Math.Ceiling(WindowSize.X * screenPercent);
            var incrY = (int) Math.Ceiling(WindowSize.Y * screenPercent);
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
            foreach (var t in subCol) t.Render();

            if (!debug || !isCollisionSystem) return;
            foreach (var rect in rects.Values) rect.DrawHallowRect(Color.GREEN);
        }

        public void SubscribeCollider(Collider c) => addQueue.Add(c);
        public void UnSubscribeCollider(Collider c) => removeQueue.Add(c);

        public void Sort(float deltaTime)
        {
            for (var i = 0; i < subCol.Count; i++)
            {
                foreach (var t in keys.Where(t => subCol[i].SampleCollision(rects[t])))
                    collisionGrid[t].Add(i);

                if (subCol[i].velocity != Vector2.Zero) subCol[i].Position += subCol[i].velocity * deltaTime;
                subCol[i].Update();
            }
        }

        public void CollDetect()
        {
            var lists = collisionGrid.Values.Where(l => l.Count > 1).ToList();
            if (!lists.Any()) return;
            foreach (var list in lists.Where(list => list.Any()))
            {
                for (var i = 0; i < list.Count - 1; i++)
                for (var j = i + 1; j < list.Count; j++)
                    subCol[list[i]].DoCollision(subCol[list[j]]);
            }
        }
    }
}