using RayWrapper.Imgui.Widgets;
using RayWrapper.Vars;
using ZimonIsHimUtils.ExtensionMethods;

namespace RayWrapper.Imgui;

public abstract class ImguiLoop : GameLoop
{
    private static bool _hasInit;
    private readonly List<IWBase> _registry = new();

    public override void Init()
    {
        if (!_hasInit)
        {
            RlImgui.Setup();
            _hasInit = true;
        }

        NormalInit();
    }

    public abstract void NormalInit();

    public override void UpdateLoop()
    {
        NormalUpdateLoop();
        UpdateReg();
    }

    public override void RenderLoop()
    {
        NormalRenderLoop();
        RlImgui.Begin();
        ImguiRenderLoop();
        RenderReg();
        RlImgui.End();
    }

    public virtual void NormalUpdateLoop()
    {
    }

    public virtual void NormalRenderLoop()
    {
    }

    public virtual void ImguiRenderLoop()
    {
    }

    public void UpdateReg() => _registry.Each(a => a.Update());
    public void RenderReg() => _registry.Each(a => a.Render());
    public void RegisterWidget(IWBase igo) => _registry.Add(igo);
    public void RegisterWidgets(params IWBase[] igo) => _registry.AddRange(igo);
    public void DeregisterWidget(IWBase igo) => _registry.Remove(igo);
    public void DeregisterWidgets(params IWBase[] igo) => _registry.RemoveAll(igo.Contains);
    public IWBase[] GetRegistry() => _registry.ToArray();
}