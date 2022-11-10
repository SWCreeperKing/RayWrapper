using RayWrapper.LegacyUI.UI;
using RayWrapperTester.Example_Setup;

namespace RayWrapperTester.Examples;

[Example("PercentBars & Sliders")]
public class PercentSliders : Example
{
    private float _percent;
    
    public PercentSliders(string tabName) : base(tabName)
    {
        var pb = new ProgressBar(120, 100, 400, 30, () => _percent);
        var slider = new Slider(100, 300, 400, 30);
        
        RegisterGameObj(pb, slider);
    }

    protected override void UpdateCall(float dt)
    {
        _percent += .005f;
        _percent %= 1;
    }
}