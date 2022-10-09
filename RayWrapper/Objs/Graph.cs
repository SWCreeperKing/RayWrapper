using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RayWrapper.Base;
using RayWrapper.Base.GameObject;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using Rectangle = Raylib_CsLo.Rectangle;

namespace RayWrapper.Objs;

public class Graph : GameObject
{
    public readonly Vector2 neg = new(-1, -1);

    public Rectangle rect;
    public ColorModule lineColor = DARKBLUE;
    public int thickness = 3;
    public int range = -1;
    public bool useBezier = false;
    public Actionable<float> maxConstraint = float.MaxValue;
    public Actionable<float> minConstraint = float.MinValue;
    public Tooltip tooltip;

    private readonly List<Vector2> _values = new();
    private readonly IDictionary<Vector2, Vector2> _realVal = new Dictionary<Vector2, Vector2>();
    private Vector2[] _cacheValues;
    private Vector2 _closest = new(-1, -1);

    public Graph(Rectangle rect)
    {
        this.rect = rect;
        tooltip = new GameBox.DefaultTooltip(new Actionable<string>(() => _realVal[_closest].ToString()));
    }

    protected override void UpdateCall()
    {
        if (!rect.IsMouseIn())
        {
            _closest = neg;
            return;
        }

        var mouse = GameBox.mousePos;
        _closest = _cacheValues.Select(v2 => (pos: v2, dist: Vector2.Distance(mouse, v2))).MinBy(t => t.dist).pos;
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

        grow.DrawHallowRect(BLACK);
        if (_closest == neg) return;
        _closest.DrawCircle(3);
        if (_realVal.ContainsKey(_closest)) tooltip.Draw();
    }

    protected override Vector2 GetPosition() => rect.Pos();
    protected override Vector2 GetSize() => rect.Size();
    protected override void UpdatePosition(Vector2 newPos) => rect.MoveTo(newPos);
    protected override void UpdateSize(Vector2 newSize) => rect.SetSize(newSize);

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
        var scaling = (max - min) / rect.Size();
            
        _cacheValues = val.Select(v2 =>
                (v2 - min) / scaling * new Vector2(1, -1) + rect.Pos() + new Vector2(0, rect.height))
            .ToArray();
            
        for (var i = 0; i < val.Count; i++)
        {
            if (!_realVal.ContainsKey(_cacheValues[i]))
            {
                _realVal.Add(_cacheValues[i], val[i]);
            }
        }
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