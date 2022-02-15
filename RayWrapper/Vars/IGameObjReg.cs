namespace RayWrapper.Vars
{
    public interface IGameObjReg
    {
        public void UpdateReg();
        public void RenderReg();
        public void RegisterGameObj(params GameObject[] igo);
        public void DeregisterGameObj(GameObject igo);
        public GameObject[] GetRegistry();
    }
}