using System.Numerics;
using RayWrapper.Base.GameObject;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.Raylib;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.UI;

public class ScrollView : GameObject
{
    private readonly Scrollbar _yScroll;
    private readonly Scrollbar _xScroll;
    private readonly List<IGameObject> _gos = new();
    private IList<IGameObject> _renderList = new List<IGameObject>();
    private Rectangle _rect;
    private Vector2 _posOffset = Vector2.Zero;
    private Vector2 _trueSize;

    public ScrollView(Rectangle rect)
    {
        _rect = new Rectangle(rect.X + 20, rect.Y, rect.W - 20, rect.H - 20);
        _yScroll = new Scrollbar(new Rectangle(rect.X, rect.Y, 20, rect.H - 20));
        _xScroll = new Scrollbar(new Rectangle(rect.X + 20, rect.Y + rect.H - 20, rect.W - 20, 20))
            { isVertical = false };
        pos = _rect.Pos;
        _trueSize = size = _rect.Size;
        _xScroll.amountInvoke = () => Math.Abs(_trueSize.X - size.X) + 1;
        _yScroll.amountInvoke = () => Math.Abs(_trueSize.Y - size.Y) + 1;
        _xScroll.OnMoveEvent += _ => Recalc();
        _yScroll.OnMoveEvent += _ => Recalc();
    }

    protected override void UpdateCall(float dt)
    {
        if (!_gos.Any()) return;
        if (_trueSize.X >= size.X) _xScroll.Update(dt);
        if (_trueSize.Y >= size.Y) _yScroll.Update(dt);
        foreach (var obj in _renderList) obj.Update(dt);
    }

    protected override void RenderCall()
    {
        if (!_gos.Any()) return;

        _rect.MaskDraw(() => { _renderList.Each(obj => obj.Render()); });

        if (_trueSize.X >= size.X) _xScroll.Render();
        if (_trueSize.Y >= size.Y) _yScroll.Render();
        _rect.DrawLines(BLACK, 1);
    }

    protected override void UpdatePosition(Vector2 newPos)
    {
        _rect.Pos = newPos;
        _yScroll.Position = new Vector2(_rect.X, _rect.Y);
        _xScroll.Position = new Vector2(_rect.X + 20, _rect.Y + _rect.H - 20);
    }

    public void Recalc()
    {
        if (!_gos.Any()) return;
        foreach (var go in _renderList) go.SetPositionAsReserveV2();
        var pos = Position;
        foreach (var go in _gos) go.Update(0);
        var v2s = _gos.Select(g => g.Position - pos + g.Size);
        _trueSize = new Vector2(Math.Max(_trueSize.X, v2s.Max(g => g.X) + 6),
            Math.Max(_trueSize.Y, v2s.Max(g => g.Y) + 6));
        _posOffset = new Vector2(_xScroll.Value, _yScroll.Value) - new Vector2(3);
        var tempRect = new Rectangle(_rect.X + _posOffset.X, _rect.Y + _posOffset.Y, _rect.W, _rect.H);
        _renderList.Clear();
        _renderList = _gos.Where(g => CheckCollisionRecs(g.GetRect(), tempRect)).ToList();
        foreach (var go in _renderList) go.Position -= _posOffset;
    }

    public void AddObj(params IGameObject[] objs)
    {
        var pos = Position;

        foreach (var t in objs)
        {
            t.Position += pos;
            t.ReserveV2();
        }

        _gos.AddRange(objs);
        Recalc();
    }
}