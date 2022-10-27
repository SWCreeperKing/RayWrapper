namespace RayWrapper.Imgui.Widgets.Base;

public interface IWidgetRegister
{
    void UpdateReg();
    void RenderReg();
    void RegisterWidgets(params IWidget[] igo);
    void DeregisterWidget(IWidget igo);
    IWidget[] GetRegistry();
}