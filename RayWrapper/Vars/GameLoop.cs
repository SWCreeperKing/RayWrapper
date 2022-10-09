using RayWrapper.Base.GameObject;

namespace RayWrapper.Vars;

public abstract class GameLoop : GameObjReg
{
    public void Update()
    {
        UpdateLoop();
        UpdateReg();
    }

    public void Render()
    {
        RenderLoop();
        RenderReg();
    }

    public abstract void Init();
    public abstract void UpdateLoop();
    public abstract void RenderLoop();

    public virtual void Dispose()
    {
    }
}