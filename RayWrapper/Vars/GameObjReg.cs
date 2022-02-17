using System.Collections.Generic;

namespace RayWrapper.Vars
{
    public abstract class GameObjReg : IGameObjReg
    {
        // TODO: Look into the GameObject & GameObjReg system. This should not use the GameObject class since GameObjReg is the superclass of that class.
        private List<IGameObject> _registry = new();

        public void UpdateReg()
        {
            foreach (var a in _registry) a.Update();
        }

        public void RenderReg()
        {
            foreach (var a in _registry) a.Render();
        }

        public void RegisterGameObj(params IGameObject[] igo) => _registry.AddRange(igo);
        public void DeregisterGameObj(IGameObject igo) => _registry.Remove(igo);
        public IGameObject[] GetRegistry() => _registry.ToArray();
    }
}