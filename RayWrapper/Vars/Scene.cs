using RayWrapper.Base.GameObject;

namespace RayWrapper.Vars;

public class Scene : GameObjReg
{
    public void Update() => UpdateReg();
    public void Render() => RenderReg();
}