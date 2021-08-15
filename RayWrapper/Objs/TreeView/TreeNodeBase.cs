using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Objs.TreeView
{
    public abstract class TreeNodeBase
    {
        public (Color unMarked, Color marked) markColor = (new(0, 0, 0, 255), new(255, 255, 255, 255));
        public abstract string Draw(Dictionary<string, TreeNode> nodes, Vector2 change, float scale);
    }
}