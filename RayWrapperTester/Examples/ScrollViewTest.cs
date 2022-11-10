using System.Numerics;
using RayWrapper.LegacyUI.UI;
using RayWrapperTester.Example_Setup;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapperTester.Examples;

[Example("ScrollView")]
public class ScrollViewTest : Example
{
    public ScrollViewTest(string tabName) : base(tabName)
    {
        ScrollView sv = new(new Rectangle(250, 150, 700, 500));
        Label l = new(Vector2.Zero, "Text1");
        Label l2 = new(new Vector2(500), "Text2");
        Label l3 = new(new Vector2(1000, 100), "Text3");
        sv.AddObj(l);
        sv.AddObj(l2);
        sv.AddObj(l3);
        
        RegisterGameObj(sv);
    }
}