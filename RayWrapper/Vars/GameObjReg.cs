using System.Collections.Generic;
using RayWrapper.Var_Interfaces;
using ZimonIsHimUtils.ExtensionMethods;

namespace RayWrapper.Vars
{
    public abstract class GameObjReg : IGameObjReg
    {
        // TODO: Look into the GameObject & GameObjReg system. This should not use the GameObject class since GameObjReg is the superclass of that class.
        private List<IGameObject> _registry = new();

        public void UpdateReg()
        {
            _registry.Each(a => a.Update());
        }

        public void RenderReg()
        {
            _registry.Each(a => a.Render());
        }

        public void RegisterGameObj(params IGameObject[] igo) => _registry.AddRange(igo);
        public void DeregisterGameObj(IGameObject igo) => _registry.Remove(igo);
        public IGameObject[] GetRegistry() => _registry.ToArray();
    }
}