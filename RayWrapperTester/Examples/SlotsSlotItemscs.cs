using System.Numerics;
using RayWrapper.LegacyUI.Slot;
using RayWrapper.LegacyUI.UI;
using RayWrapperTester.Example_Setup;
using static Raylib_CsLo.Raylib;

namespace RayWrapperTester.Examples;

[Example("Slot Test")]
public class SlotTest : Example
{
    public SlotTest(string tabName) : base(tabName)
    {
        RectItem ri = new(new Vector2(400), new Vector2(75)) { slotDependent = false, id = "blue", color = BLUE };
        CircleItem ci = new(new Vector2(400, 475), new Vector2(75)) { color = RED, id = "red" };
        ImageObj io = new("Assets/Images/Untitled.png", new Vector2(400, 550));
        ImageObj io2 = new("Assets/Images/Untitled.png", new Vector2(1025, 300));

        io.Size = io2.Size = new Vector2(75);
        io2.ImageAlpha = 100;

        ImageItem ii = new(io) { slotDependent = false };
        Slot sl = new(new Vector2(800, 300), new Vector2(75)) { color = PURPLE };
        Slot sl1 = new(new Vector2(875, 300), new Vector2(75)) { color = BLUE, idRestriction = "blue" };
        Slot sl2 = new(new Vector2(950, 300), new Vector2(75)) { color = RED, idRestriction = "red" };

        ImageSlot imSl = new(io2) { color = YELLOW };
        ii.SetSlot(imSl);
        
        RegisterGameObj(imSl, ri, ci, ii, sl, sl1, sl2);
    }
}