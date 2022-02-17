﻿namespace RayWrapper.Vars
{
    public interface IGameObjReg
    {
        void UpdateReg();
        void RenderReg();
        void RegisterGameObj(params IGameObject[] igo);
        void DeregisterGameObj(IGameObject igo);
        IGameObject[] GetRegistry();
    }
}