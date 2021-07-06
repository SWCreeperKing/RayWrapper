using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RayWrapper.Animation
{
    public class AnimationStep
    {
        // actual animation control
        public long lastTime = -1;
        public long duration;
        public long timeAccumulated;
        private List<(string shapeId, string op, string[] args)> _operations = new();

        public AnimationStep(float seconds) => duration = (long) (seconds * 1000);

        public bool Execute(AnimationBuilder ab)
        {
            var time = GameBox.GetTimeMs();
            if (lastTime == -1)
            {
                lastTime = time;
                return false;
            }
            var deltaTime = time - lastTime;
            timeAccumulated += deltaTime;
            var snap = timeAccumulated >= duration;
            var timeFactor = (float) deltaTime / duration;

            foreach (var (id, op, args) in _operations)
            {
                var shape = ab.GetShapeWithId(id);
                if (shape is null) continue;
                switch (op)
                {
                    case "grow" or "shrink" or "slide" or "move":
                        var v2 = new Vector2(float.Parse(args[0]), float.Parse(args[1]));
                        if (snap) shape.Update(deltaTime, timeFactor, op, new[] {$"{v2.X}", $"{v2.Y}"}, true);
                        else
                        {
                            v2 *= timeFactor;
                            shape.Update(deltaTime, timeFactor, op, new[] {$"{v2.X}", $"{v2.Y}"}, false);
                        }

                        break;
                    default:
                        shape.Update(deltaTime, timeFactor, op, args, snap);
                        break;
                }
                
            }

            if (snap)
            {
                foreach (var id in ab.GetShapeIds())
                {
                    var shape = ab.GetShapeWithId(id);
                    ab.SetShape(id, shape.endState);
                }
            }

            lastTime = GameBox.GetTimeMs();
            return snap;
        }

        public AnimationStep AddCustomOp(string shapeId, string op, params string[] args)
        {
            _operations.Add((shapeId, op, args));
            return this;
        }

        public AnimationStep CopyStep() =>
            new(duration / 1000f)
            {
                _operations = _operations.ToList(), lastTime = lastTime, timeAccumulated = timeAccumulated
            };
    }
}