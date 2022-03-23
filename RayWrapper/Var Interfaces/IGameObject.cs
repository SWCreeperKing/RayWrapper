using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.Var_Interfaces
{
    public interface IGameObject
    {
        Vector2 Position { get; set; }
        Vector2 Size { get; }

        void Update();
        void Render();
        
        // TODO: Remove if not appropriate in interface.
        float FullLength { get; }
        float FullHeight { get; }

        // TODO: Remove if not appropriate in interface.
        Rectangle GetDebugRect();
        void ReserveV2();
        Vector2 GetReserveV2();
        void SetPositionAsReserveV2();
    }
}