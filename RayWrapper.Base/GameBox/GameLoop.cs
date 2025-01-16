using RayWrapper.Base.GameObject;

namespace RayWrapper.Base.GameBox;

public abstract class GameLoop : TypeRegister<IGameObject>
{
    public void Update(float dt)
    {
        UpdateLoop(dt);
        UpdateReg(dt);
    }

    public void Render()
    {
        RenderLoop();
        RenderReg();
    }

    public void Dispose()
    {
        DisposeLoop();
        DisposeReg();
    }
    
    public void UpdateReg(float dt) => register.ForEach(o => o.Update(dt));
    public void RenderReg() => register.ForEach(o => o.Render());
    public void DisposeReg() => register.ForEach(o => o.Dispose());
    public abstract void Init();
    public abstract void UpdateLoop(float dt);
    public abstract void RenderLoop();

    public virtual void DisposeLoop()
    {
    }
}