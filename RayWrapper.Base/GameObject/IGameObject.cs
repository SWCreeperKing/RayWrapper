using System.Numerics;
using Raylib_CsLo;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.Base.GameObject;

public interface IGameObject
{
    Vector2 Position { get; set; }
    Vector2 Size { get; }

    void Update(float dt);
    void Render();
    void Dispose();

    // TODO: Remove if not appropriate in interface.
    Raylib_CsLo.Rectangle GetRawRect();
    Rectangle GetRect();
    void ReserveV2();
    Vector2 GetReserveV2();
    void SetPositionAsReserveV2();

    MouseCursor GetOccupiedCursor();
}