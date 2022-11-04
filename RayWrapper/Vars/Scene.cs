using RayWrapper.Base.GameObject;
using ZimonIsHimUtils.ExtensionMethods;

namespace RayWrapper.Vars;

public class Scene : TypeRegister<IGameObject>
{
    public void Update() => register.Each(o => o.Update());
    public void Render() => register.Each(o => o.Render());
}