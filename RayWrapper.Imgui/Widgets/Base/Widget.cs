namespace RayWrapper.Imgui.Widgets.Base;

public abstract class Widget : WidgetRegister, IWidget
{
    public void Update()
    {
        UpdateCall();
        UpdateReg();
    }

    public void Render()
    {
        RenderCall();
        RenderReg();
    }

    public void Dispose()
    {
        DisposeCall();
        DisposeReg();
    }

    public void UpdateRaw() => UpdateCall();
    public void RenderRaw() => RenderCall();
    public void DisposeRaw() => DisposeCall();

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