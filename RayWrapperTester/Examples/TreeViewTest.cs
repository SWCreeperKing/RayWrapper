using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.Primitives;
using RayWrapper.LegacyUI.TreeView;
using RayWrapper.LegacyUI.TreeView.TreeNodeChain;
using RayWrapper.LegacyUI.TreeView.TreeNodeChain.NodeShapes;
using RayWrapper.LegacyUI.UI;
using RayWrapperTester.Example_Setup;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

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
            new ImageNode(new ImageObj("Assets/Images/Untitled.png"), new Vector2(10, 2), new Vector2(4), "Image :D")
        );

        nodeChain.AddBranch(new NodeChain(
                new Ball(new Vector2(4, 8), new Vector2(2, 1), "yeet")
                {
                    completed = new Actionable<bool>(() =>
                    {
                        return Raylib.IsKeyDown(KeyboardKey.KEY_SPACE);
                    })
                }),
            nodeChain.nodes.Count - 2);

        var mask = new Rectangle(Vector2.Zero, GameBox.WindowSize);
        mask.ExtendPos(0, -60);
        TreeView tv = new(nodeChain)
        {
            axisOffset = new Vector2(5, 5), verticalMovement = false,
            bounds = new Rectangle(0, 0, 15, 0),
            mask = mask
        };

        RegisterGameObj(tv);
    }
}