
using ZimonIsHimUtils.ExtensionMethods;

namespace RayWrapper.Base.GameObject;

public abstract class GameObjReg : IGameObjReg
{
    // TODO: Look into the GameObject & GameObjReg system. This should not use the GameObject class since GameObjReg is the superclass of that class.
    public readonly List<IGameObject> registry = new();

    public void UpdateReg() => registry.Each(a => a.Update());
    public void RenderReg() => registry.Each(a => a.Render());
    public void RegisterGameObj(IGameObject igo) => registry.Add(igo);
    public void RegisterGameObj(params IGameObject[] igo) => registry.AddRange(igo);
    public void DeregisterGameObj(IGameObject igo) => registry.Remove(igo);
    public void DeregisterGameObj(params IGameObject[] igo) => registry.RemoveAll(igo.Contains);
    public IGameObject[] GetRegistry() => registry.ToArray();
}