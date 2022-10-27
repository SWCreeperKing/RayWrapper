using ZimonIsHimUtils.ExtensionMethods;

namespace RayWrapper.Imgui.Widgets.Base;

public abstract class WidgetRegister : IWidgetRegister
{
    private readonly List<IWidget> _registry = new();

    public void UpdateReg() => _registry.Each(a => a.Update());
    public void RenderReg() => _registry.Each(a => a.Render());
    public void DisposeReg() => _registry.Each(a => a.Dispose());
    public void RegisterWidget(IWidget igo) => _registry.Add(igo);
    public void RegisterWidgets(params IWidget[] igo) => _registry.AddRange(igo);
    public void DeregisterWidget(IWidget igo) => _registry.Remove(igo);
    public void DeregisterWidgets(params IWidget[] igo) => _registry.RemoveAll(igo.Contains);
    public IWidget[] GetRegistry() => _registry.ToArray();
}