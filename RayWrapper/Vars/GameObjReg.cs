using System.Collections.Generic;

namespace RayWrapper.Vars
{
    public abstract class GameObjReg
    {
        private List<GameObject> _registry = new();
        
        public void UpdateReg()
        {
            foreach (var a in _registry) a.Update();
        }

        public void RenderReg()
        {
            foreach (var a in _registry) a.Render();
        }
        
        public void RegisterGameObj(params GameObject[] igo) => _registry.AddRange(igo);
        public void DeregisterGameObj(GameObject igo) => _registry.Remove(igo);
        public GameObject[] GetRegistry() => _registry.ToArray();
    }
}