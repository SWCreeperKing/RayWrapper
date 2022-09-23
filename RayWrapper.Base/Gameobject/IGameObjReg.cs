namespace RayWrapper.Base.Gameobject;

public interface IGameObjReg
{
    void UpdateReg();
    void RenderReg();
    void RegisterGameObj(params IGameObject[] igo);
    void DeregisterGameObj(IGameObject igo);
    IGameObject[] GetRegistry();
}