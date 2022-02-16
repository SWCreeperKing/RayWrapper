namespace RayWrapper.Vars
{
    public interface IGameObjReg
    {
        public void UpdateReg();
        public void RenderReg();
        public void RegisterGameObj(params IGameObject[] igo);
        public void DeregisterGameObj(IGameObject igo);
        public IGameObject[] GetRegistry();
    }
}