using RayWrapper.Vars;

namespace RayWrapper.Imgui;

public abstract class ImguiLoop : GameLoop
{
    private static bool _hasInit;

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

    public override void RenderLoop()
    {
        NormalRenderLoop();
        RlImgui.Begin();
        ImguiRenderLoop();
        RlImgui.End();
    }

    public abstract void NormalRenderLoop();
    public abstract void ImguiRenderLoop();
}