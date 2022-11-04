using RayWrapper.Base.GameObject;
using ZimonIsHimUtils.ExtensionMethods;

namespace RayWrapper.Vars;

public abstract class GameLoop : TypeRegister<IGameObject>
{
    public void Update()
    {
        UpdateLoop();
        register.Each(o => o.Update());
    }

    public void Render()
    {
        RenderLoop();
        register.Each(o => o.Render());
    }

    public abstract void Init();
    public abstract void UpdateLoop();
    public abstract void RenderLoop();

    public virtual void Dispose()
    {
    }
}