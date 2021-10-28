namespace RayWrapper.Objs
{
    public abstract class Scene
    {
        protected bool isInit;

        public void Init()
        {
            if (isInit) return;
            InitCall();
            isInit = true;
        }

        public virtual void InitCall()
        {
        }
        
        public abstract void UpdateCall();
        public abstract void RenderCall();
    }
}