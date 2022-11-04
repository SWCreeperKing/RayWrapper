namespace RayWrapper.Base.GameObject;

public interface ITypeRegister<T>
{
    void RegisterGameObj(T t);
    void RegisterGameObj(params T[] ts);
    void DeregisterGameObj(T t);
    void DeregisterGameObj(params T[] ts);
    T[] GetRegistry();
}