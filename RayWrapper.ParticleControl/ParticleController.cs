using System.Numerics;
using RayWrapper.Base.GameObject;
using ZimonIsHimUtils.ExtensionMethods;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;
using static RayWrapper.Base.Extras.Core;

namespace RayWrapper.ParticleControl;

public class ParticleController : GameObject
{
    public List<Particle.Data> particleData = new();
    
    private List<Particle> _particles = new();

    protected override void UpdateCall(float dt)
    {
        _particles.Each(p => p.Update());
        _particles = _particles.Where(p => !p.IsDead()).ToList();
    }

    protected override void RenderCall()
    {
        _particles.Each(p => p.Draw());
    }

    public void CreateParticle(int index)
    {
        var (atlas, id, loc, size, lifeTime, gravity, expansion,
            fadeOut, origin, rotation, rps, color) = particleData[index];

        _particles.Add(new Particle(loc, size, (long) lifeTime, gravity!, expansion!, fadeOut, origin!, 
            rotation, rps, color, atlas, id));
    }
}

public class Ranger
{
    public double d;
    public double? endD;

    public static implicit operator Ranger(int i) => new() { d = i };
    public static implicit operator Ranger(float f) => new() { d = f };
    public static implicit operator Ranger(double d) => new() { d = d };
    public static implicit operator Ranger(Range r) => new() { d = r.Start.Value, endD = r.End.Value };
    public static implicit operator double(Ranger r) => r.endD is null ? r.d : GameBox.Random.Next(r.d, r.endD!.Value);
}

public class VectorRanger
{
    public Vector2 v2;
    public Vector2? endV2;

    public static implicit operator VectorRanger(Vector2 v2) => new() { v2 = v2 };
    public static implicit operator VectorRanger((Vector2 v21, Vector2 v22) v) => new() { v2 = v.v21, endV2 = v.v22 };
    public static implicit operator VectorRanger(Rectangle r) => new() { v2 = r.Pos, endV2 = r.Pos + r.Size };

    public static implicit operator Vector2(VectorRanger vr) =>
        vr.endV2 is null ? vr.v2 : GameBox.Random.Next(vr.v2, vr.endV2!.Value);
}