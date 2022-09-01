using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs;

namespace RayWrapper.ParticleControl;

public class Particle
{
    public Vector2 pos;
    public Vector2 size;
    public float lifeTime;
    public Vector2 gravity;
    public Vector2 expansion;
    public bool fadeOut;
    public Vector2 origin;
    public float rotation;
    public float rps;
    public Color baseColor;
    public TextureAtlas atlas;
    public string textureId;

    private long _lastUpdate;
    private float _startLifeTime;

    public record Data(TextureAtlas Atlas, string TextureId, VectorRanger SpawnLocation, VectorRanger Size,
        Ranger LifeTime, VectorRanger? Gravity = null, VectorRanger? Expansion = null, bool FadeOut = true,
        VectorRanger? Origin = null, Ranger? Rotation = null, Ranger? Rps = null, Color? BaseColor = null);

    public Particle(Vector2 pos, Vector2 size, long lifeTime, VectorRanger? gravity, VectorRanger? expansion,
        bool fadeOut,
        VectorRanger? origin, Ranger? rotation, Ranger? rps, Color? baseColor, TextureAtlas atlas, string textureId)
    {
        this.pos = pos;
        this.size = size;
        this.gravity = gravity ?? Vector2.Zero;
        this.expansion = expansion ?? Vector2.Zero;
        this.fadeOut = fadeOut;
        this.origin = origin ?? Vector2.Zero;
        this.rotation = (float) (rotation ?? 0);
        this.rps = (float) (rps ?? 0);
        this.baseColor = baseColor ?? Raylib.WHITE;
        this.atlas = atlas;
        this.textureId = textureId;
        _startLifeTime = this.lifeTime = lifeTime;
        _lastUpdate = GameBox.GetTimeMs();
    }

    public void Update()
    {
        var thisTime = GameBox.GetTimeMs();
        var dt = (thisTime - _lastUpdate) / 1000f;

        pos += gravity * dt;
        size += expansion * dt;
        rotation += rps * dt;
        lifeTime -= dt;

        _lastUpdate = thisTime;
    }

    public void Draw()
    {
        var c = fadeOut ? baseColor.SetAlpha((int) (baseColor.a * (lifeTime / _startLifeTime))) : baseColor;
        atlas.Draw(textureId, pos.X, pos.Y, size.X, size.Y, origin, rotation, c);
    }

    public bool IsDead() => lifeTime <= 0;
}