﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class Graph : GameObject
    {
        public readonly Vector2 neg = new(-1, -1);

        public override Vector2 Position
        {
            get => rect.Pos();
            set => rect = rect.MoveTo(value);
        }

        public override Vector2 Size => rect.Size();

        public Rectangle rect;
        public ColorModule lineColor = Color.DARKBLUE;
        public int thickness = 3;
        public int range = -1;
        public bool useBezier = false;
        public Actionable<float> maxConstraint = float.MaxValue;
        public Actionable<float> minConstraint = float.MinValue;

        private readonly List<Vector2> _values = new();
        private readonly Dictionary<Vector2, Vector2> _realVal = new();
        private Vector2[] _cacheValues;
        private Vector2 closest = new(-1, -1);

        public Graph(Rectangle rect) => this.rect = rect;

        protected override void UpdateCall()
        {
            if (!rect.IsMouseIn())
            {
                closest = neg;
                return;
            }

            var mouse = GameBox.mousePos;
            closest = _cacheValues.Select(v2 => (pos: v2, dist: Vector2.Distance(mouse, v2))).OrderBy(t => t.dist)
                .First().pos;
        }

        protected override void RenderCall()
        {
            var grow = rect.Grow(4);
            if (_cacheValues.Length < 2) return;
            grow.MaskDraw(() =>
            {
                if (useBezier) _cacheValues.DrawArrAsBezLine(lineColor, thickness);
                else _cacheValues.DrawArrAsLine(lineColor, thickness); 
            });
            grow.DrawHallowRect(Color.BLACK);
            if (closest == neg) return;
            closest.DrawCircle(3);
            if (_realVal.ContainsKey(closest)) GameBox.tooltip.Add(_realVal[closest].ToString());
        }

        public void UpdateVal()
        {
            if (_values.Count < 1) return;
            var val = (range != -1 && _values.Count > range
                ? _values.ToArray()[(_values.Count - range)..]
                : (IEnumerable<Vector2>)_values).ToList();
            var minC = (float) minConstraint;
            var maxC = (float) maxConstraint;
            var minCalc = val.Where(v2 => v2.X > minC && v2.Y > minC);
            var maxCalc = val.Where(v2 => v2.X < maxC && v2.Y < maxC);
            var min = new Vector2(minCalc.Min(v => v.X), minCalc.Min(v => v.Y));
            var max = new Vector2(maxCalc.Max(v => v.X), maxCalc.Max(v => v.Y));
            var size = max - min;
            var scaling = size / rect.Size();
            _cacheValues = val.Select(v2 =>
                    (v2 - min) / scaling * new Vector2(1, -1) + rect.Pos() + new Vector2(0, rect.height))
                .ToArray();
            for (var i = 0; i < val.Count; i++)
                if (!_realVal.ContainsKey(_cacheValues[i]))
                    _realVal.Add(_cacheValues[i], val[i]);
        }

        public void NewVal(params Vector2[] v)
        {
            Clear();
            AddVal(v);
        }

        public void AddVal(params Vector2[] v)
        {
            _values.AddRange(v.Select(v => v.FixVector()));
            UpdateVal();
        }

        public void RemoveVal(Vector2 v)
        {
            _values.Remove(v);
            UpdateVal();
        }

        public void Clear()
        {
            _values.Clear();
            UpdateVal();
        }

        public void ExecuteFunction(Func<float, Vector2> func, int start = 0, int count = 500, float dx = .1f) =>
            NewVal(Enumerable.Range(start, count).Select(i => func.Invoke(i * dx)).ToArray());

        public int Amount() => _values.Count;
    }
}