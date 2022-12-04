using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.LegacyUI.UI;
using RayWrapper.ParticleControl;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

new GameBox(new RayWrapperTesterParticle.Program(), new Vector2(1280, 720));

namespace RayWrapperTesterParticle
{
    public partial class Program : GameLoop
    {
        public Rectangle spawnArea = new(600, 500, 50, 50);
        public ParticleController pc;

        public override void Init()
        {
            var ta = new TextureAtlas("Assets/Images/A.png", 16);
            ta.Register("p1", 0, 0);

            pc = new ParticleController();
            pc.particleData.Add(new Particle.Data(ta, "p1", spawnArea, new Vector2(48), 
                1..3, new Vector2(0, -5), Rotation:..360));
            RegisterGameObj(pc);
        }

        public override void UpdateLoop(float dt)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) pc.CreateParticle(0);
        }

        public override void RenderLoop()
        {
            spawnArea.Draw(Raylib.BLUE);
        }
    }
}