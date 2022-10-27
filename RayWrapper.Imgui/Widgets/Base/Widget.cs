namespace RayWrapper.Imgui.Widgets.Base;

public abstract class Widget : WidgetRegister, IWidget
{
    public void Update() => UpdateCall();
    public void Render() => RenderCall();
    public void Dispose() => DisposeCall();

    public virtual void UpdateCall()
    {
    }

    public virtual void RenderCall()
    {
    }

    public virtual void DisposeCall()
    {
    }
}