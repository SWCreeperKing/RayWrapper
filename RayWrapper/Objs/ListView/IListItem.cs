using System.Numerics;

namespace RayWrapper.Objs.ListView;

public interface IListItem
{
    /// <summary>
    /// how big the size of the list should be
    /// </summary>
    public int ListSize();

    /// <summary>
    /// how big the item size is, only called once and at init!
    /// </summary>
    public Vector2 ItemSize();

    /// <summary>
    /// renders, but also wants the <see cref="IListItem.Size"/> as a return 
    /// </summary>
    public void Render(Vector2 offset, int item, bool isMouseActive);
}