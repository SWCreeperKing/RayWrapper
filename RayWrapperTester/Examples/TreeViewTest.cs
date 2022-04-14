using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Objs;
using RayWrapper.Objs.TreeView;
using RayWrapper.Objs.TreeView.TreeNodeChain;
using RayWrapper.Objs.TreeView.TreeNodeChain.NodeShapes;
using RayWrapperTester.Example_Setup;
using static RayWrapper.RectWrapper;

namespace RayWrapperTester.Examples;

[Example("TreeView")]
public class TreeViewTest : Example
{
    public TreeViewTest(string tabName) : base(tabName)
    {
        var nodeChain = new NodeChain(
            new Box(Vector2.One, "hi") { completed = true },
            new Box(new Vector2(1, 3), "hi2") { completed = true },
            new Ball(new Vector2(3, 1), "hi3") { completed = true },
            new Ball(new Vector2(3, 3), "hi4") { completed = true },
            new ImageNode(new ImageObj("Images/Untitled.png"), new Vector2(10, 2), new Vector2(4), "Image :D")
        );
        
        nodeChain.AddBranch(new NodeChain(new Ball(new Vector2(4, 8), new Vector2(2, 1), "yeet")),
            nodeChain.nodes.Count - 2);
        
        TreeView tv = new(nodeChain)
        {
            axisOffset = new Vector2(5, 5), verticalMovement = false,
            bounds = new Rectangle(0, 0, 15, 0),
            mask = AssembleRectFromVec(Vector2.Zero, GameBox.WindowSize).ExtendPos(new Vector2(0, -60))
        };
        
        RegisterGameObj(tv);
    }
}