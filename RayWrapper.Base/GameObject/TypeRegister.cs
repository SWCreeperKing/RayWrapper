namespace RayWrapper.Base.GameObject;

public class TypeRegister<T> : ITypeRegister<T>
{
    public readonly List<T> register = new();

    public void RegisterGameObj(T t) => register.Add(t);
    public void RegisterGameObj(params T[] ts) => register.AddRange(ts);
    public void DeregisterGameObj(T t) => register.Remove(t);
    public void DeregisterGameObj(params T[] ts) => register.RemoveAll(ts.Contains);
    public T[] GetRegistry() => register.ToArray();
}