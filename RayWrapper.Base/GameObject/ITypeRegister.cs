namespace RayWrapper.Base.GameObject;

public interface ITypeRegister<T>
{
    void RegisterGameObj(T t);
    void RegisterGameObjs(params T[] ts);
    void DeregisterGameObj(T t);
    void DeregisterGameObjs(params T[] ts);
    T[] GetRegistry();
}