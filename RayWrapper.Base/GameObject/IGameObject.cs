using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.Base.GameObject;

public interface IGameObject
{
    Vector2 Position { get; set; }
    Vector2 Size { get; }

    void Update();
    void Render();

    // TODO: Remove if not appropriate in interface.
    Raylib_CsLo.Rectangle GetRawRect();
    Rectangle GetRect();
    void ReserveV2();
    Vector2 GetReserveV2();
    void SetPositionAsReserveV2();

    MouseCursor GetOccupiedCursor();
}