using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Animation.AnimationShapes;

namespace RayWrapper.Animation
{
    public class AnimationBuilder
    {
        private Dictionary<string, AnimationShape> _shapes = new();
        private List<AnimationStep> _steps = new();
        private int _buildingStep;
        public int _workingStep;

        public void RenderShapes()
        {
            foreach (var shape in _shapes.Values.Where(shape => shape.isVisible))
                shape.DrawShape();
        }

        public AnimationBuilder AddShape(AnimationShape shape)
        {
            if (_shapes.ContainsKey(shape.id)) return this;
            _shapes.Add(shape.id, shape);
            return this;
        }

        public AnimationShape GetShapeWithId(string id) => !_shapes.ContainsKey(id) ? null : _shapes[id];

        public AnimationStep GetNextStep()
        {
            if (IsOver()) return null;
            foreach (var shape in _shapes.Values) shape.NewState();
            _steps[_workingStep].onEnd?.Invoke(this);
            return _steps[_workingStep++];
        }

        public bool IsOver() => _workingStep >= _steps.Count;

        public AnimationBuilder CopyBuilder() =>
            new()
            {
                _steps = _steps.Select(a => a.CopyStep()).ToList(), _workingStep = _workingStep,
                _shapes = _shapes.ToDictionary(kv => kv.Key, kv => kv.Value.CopyState())
            };

        public string[] GetShapeIds() => _shapes.Keys.ToArray();

        public void SetShape(string id, AnimationShape shape)
        {
            if (_shapes.ContainsKey(id)) _shapes[id] = shape;
        }

        public static implicit operator Animation(AnimationBuilder builder) => new(builder);

        // step building

        public AnimationBuilder AddStep(float seconds)
        {
            _steps.Add(new AnimationStep(seconds));
            _buildingStep = _steps.Count - 1;
            return this;
        }

        public AnimationBuilder AddSingleStep(float seconds, string shapeId, string op, params string[] args)
        {
            AddStep(seconds);
            _steps[_buildingStep].AddCustomOp(shapeId, op, args);
            return this;
        }

        public AnimationBuilder AddCustomOp(string shapeId, string op, params string[] args)
        {
            if (_steps.Count == 0) AddStep(1);
            _steps[_buildingStep].AddCustomOp(shapeId, op, args);
            return this;
        }

        public AnimationBuilder Shrink(string shapeId, Vector2 shrinkBy)
        {
            AddCustomOp(shapeId, "shrink", $"{shrinkBy.X}", $"{shrinkBy.Y}");
            return this;
        }

        public AnimationBuilder Grow(string shapeId, Vector2 growBy)
        {
            AddCustomOp(shapeId, "grow", $"{growBy.X}", $"{growBy.Y}");
            return this;
        }

        public AnimationBuilder Slide(string shapeId, Vector2 slideBy)
        {
            AddCustomOp(shapeId, "slide", $"{slideBy.X}", $"{slideBy.Y}");
            return this;
        }

        public AnimationBuilder Slip(string shapeId, Vector2 slideBy)
        {
            AddCustomOp(shapeId, "slip", $"{slideBy.X}", $"{slideBy.Y}");
            return this;
        }

        public AnimationBuilder Move(string shapeId, Vector2 moveBy)
        {
            AddCustomOp(shapeId, "move", $"{moveBy.X}", $"{moveBy.Y}");
            return this;
        }

        public AnimationBuilder SetVisible(string shapeId, bool visible)
        {
            AddCustomOp(shapeId, "visSet", $"{visible}");
            return this;
        }

        public AnimationBuilder ToggleVisible(string shapeId)
        {
            AddCustomOp(shapeId, "visTog");
            return this;
        }

        public AnimationBuilder SetEndAction(Action<AnimationBuilder> onEnd)
        {
            if (_steps.Count == 0) AddStep(1);
            _steps[_buildingStep].onEnd = onEnd;
            return this;
        }

        public AnimationBuilder SetRunningTrigger(Action<AnimationBuilder> runningTrigger)
        {
            if (_steps.Count == 0) AddStep(1);
            _steps[_buildingStep].runningTrigger = runningTrigger;
            return this;
        }

        public AnimationBuilder WaitForTrigger(Func<AnimationBuilder, bool> trigger)
        {
            if (_steps.Count == 0) AddStep(-1);
            else _steps[_buildingStep].duration = -1;
            _steps[_buildingStep].continueTrigger = trigger;
            return this;
        }

        public Rectangle GetRectOfId(string shapeId)
        {
            if (!_shapes.ContainsKey(shapeId)) return new Rectangle(0, 0, 0, 0);
            var shape = _shapes[shapeId];
            return RectWrapper.AssembleRectFromVec(shape.pos, shape.size);
        }
    }
}