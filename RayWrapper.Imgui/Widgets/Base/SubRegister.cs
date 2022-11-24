using RayWrapper.Base.GameObject;

namespace RayWrapper.Imgui.Widgets.Base;

public abstract class SubRegister<T> : GameObject where T : SubRegister<T>
{
    public new void Update(float dt) => UpdateReg(dt);

    public new void Render()
    {
        if (!Begin()) return;
        RenderReg();
        End();
    }
    
    public new void Dispose() => DisposeReg();

    public abstract bool Begin();
    public abstract void End();
    
    public T Add(IGameObject igo)
    {
        RegisterGameObj(igo);
        return (T) this;
    }

    public T Add(params IGameObject[] igo)
    {
        RegisterGameObj(igo);
        return (T) this;
    }

    public T Remove(IGameObject igo)
    {
        DeregisterGameObj(igo);
        return (T) this;
    }

    public T Remove(params IGameObject[] igo)
    {
        DeregisterGameObj(igo);
        return (T) this;
    }
}