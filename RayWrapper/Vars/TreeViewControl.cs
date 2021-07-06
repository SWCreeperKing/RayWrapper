using System.Numerics;
using RayWrapper.TreeViewShapes;

namespace RayWrapper.Vars
{
    public abstract class TreeViewControl
    {
        public abstract TreeViewShape[] GetNodes();
        public abstract Vector2 GetPos(string id, string[] carry);
        public abstract bool GetMarked(string id, string[] carry);
        public virtual bool GetVisual(string id, string[] carry) => true;
        public virtual string GetTooltip(string id, string[] carry) => "";

        public virtual void Click(string id, string[] carry)
        {
        }
    }
}