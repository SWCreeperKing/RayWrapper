using System.Collections.Generic;

namespace RayWrapper.Vars
{
    public abstract class GameObjReg : IGameObjReg
    {
        // TODO: Look into the GameObject & GameObjReg system. This should not use the GameObject class since GameObjReg is the superclass of that class.
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