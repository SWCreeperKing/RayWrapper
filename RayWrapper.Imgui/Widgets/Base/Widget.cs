namespace RayWrapper.Imgui.Widgets.Base;

public abstract class Widget : WidgetRegister, IWidget
{
    public void Update() => UpdateCall();
    public void Render() => RenderCall();
    public void Dispose() => DisposeCall();

    protected virtual void UpdateCall()
    {
    }

    protected virtual void RenderCall()
    {
    }

    protected virtual void DisposeCall()
    {
    }
}