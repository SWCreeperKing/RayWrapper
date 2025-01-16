namespace RayWrapper.LegacyUI;

public interface IStyle<out T>
{
    public T Copy();
}