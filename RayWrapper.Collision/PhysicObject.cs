using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Collision;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;

namespace RayWrapperTester
{
    public abstract class PhysicObject : GhostObject
    {
        public enum CollisionBind
        {
            Circle,
            Rect
        }

        public override Vector2 Position
        {
            get => body.position;
            set => body.position = value;
        }

        public bool IsStatic
        {
            get => body.enabled;
            set => body.enabled = value;
        }

        public ColorModule color = RED;
        public PhysicsBodyData body;

        public abstract CollisionBind CollisionBinding { get; }

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
            DebugDraw();
        }

        public void DebugDraw()
        {
            var vertexData = body.shape.vertexData;
            var vertexCount = vertexData.vertexCount;
            for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
            {
                var v1 = body.GetVertex(vertexIndex);
                var plus1 = vertexIndex + 1;
                var v2 = body.GetVertex(plus1 == vertexCount ? 0 : plus1);
                DrawLineV(v1, v2, color);
            }
        }

        public virtual Rectangle GetRect() => RectWrapper.Zero;
        public virtual (Vector2 pos, float radius) GetCircle() => (Vector2.Zero, 0);

        ~PhysicObject()
        {
            unsafe
            {
                fixed (PhysicsBodyData* bData = &body) Physac.DestroyPhysicsBody(bData);
            }
        }
    }
}