using System.Numerics;
using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public interface ISlider
{
    public bool Render(string label, string format, ImGuiSliderFlags flags);
    public void Invoke();
}

public abstract class SliderBase<T, TA> : ISlider
{
    protected T inVar;
    protected Action<T> action;
    protected TA min;
    protected TA max;

    protected SliderBase(T inVar, Action<T> action, TA min, TA max)
    {
        this.inVar = inVar;
        this.action = action;
        this.min = min;
        this.max = max;
    }

    public abstract bool Render(string label, string format, ImGuiSliderFlags flags);
    public virtual void Invoke() => action?.Invoke(inVar);
}

public class SliderAngle : SliderBase<float, float>
{
    public SliderAngle(float inVar, Action<float> action, float min = -360, float max = 360) : base(inVar, action, min,
        max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.SliderAngle(label, ref inVar, min, max, format, flags);
    }
}

public class SliderFloat : SliderBase<float, float>
{
    public SliderFloat(float inVar, Action<float> action, float min = -500, float max = 500) : base(inVar, action, min,
        max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.SliderFloat(label, ref inVar, min, max, format, flags);
    }
}

public class SliderVector2 : SliderBase<Vector2, float>
{
    public SliderVector2(Vector2 inVar, Action<Vector2> action, float min = -500, float max = 500) : base(inVar, action,
        min,
        max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.SliderFloat2(label, ref inVar, min, max, format, flags);
    }
}

public class SliderVector3 : SliderBase<Vector3, float>
{
    public SliderVector3(Vector3 inVar, Action<Vector3> action, float min = -500, float max = 500) : base(inVar, action,
        min,
        max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.SliderFloat3(label, ref inVar, min, max, format, flags);
    }
}

public class SliderVector4 : SliderBase<Vector4, float>
{
    public SliderVector4(Vector4 inVar, Action<Vector4> action, float min = -500, float max = 500) : base(inVar, action,
        min,
        max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.SliderFloat4(label, ref inVar, min, max, format, flags);
    }
}

public class SliderInt : SliderBase<int, int>
{
    public SliderInt(int inVar, Action<int> action, int min = -500, int max = 500) : base(inVar, action, min, max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.SliderInt(label, ref inVar, min, max, format, flags);
    }
}

public class SliderInts : SliderBase<int[], int>
{
    public SliderInts(int[] inVar, Action<int[]> action, int min, int max = 500) : base(inVar, action, min, max)
    {
        if (inVar.Length is < 2 or > 4) throw new ArgumentException("InputInts array must be of length: 2, 3, or 4");
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return inVar.Length switch
        {
            2 => ImGui.SliderInt2(label, ref inVar[0], min, max, format, flags),
            3 => ImGui.SliderInt3(label, ref inVar[0], min, max, format, flags),
            4 => ImGui.SliderInt4(label, ref inVar[0], min, max, format, flags),
        };
    }
}

public abstract class VSliderBase<T> : SliderBase<T, T>
{
    protected Vector2 size;

    protected VSliderBase(T inVar, Action<T> action, Vector2 size, T min, T max) : base(inVar, action, min, max)
    {
        this.size = size;
    }
}

public class VSliderFloat : VSliderBase<float>
{
    public VSliderFloat(float inVar, Action<float> action, Vector2 size, float min = -500, float max = 500) : base(
        inVar,
        action, size, min, max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.VSliderFloat(label, size, ref inVar, min, max, format, flags);
    }
}

public class VSliderInt : VSliderBase<int>
{
    public VSliderInt(int inVar, Action<int> action, Vector2 size, int min = -500, int max = 500) : base(inVar, action,
        size,
        min, max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.VSliderInt(label, size, ref inVar, min, max, format, flags);
    }
}

public class Slider : Widget
{
    private string _label;
    private ISlider _type;
    private string _format;
    private ImGuiSliderFlags _flags;

    public Slider(string label, ISlider type, string format = "", ImGuiSliderFlags flags = ImGuiSliderFlags.None)
    {
        _label = label;
        _type = type;
        _format = format;
        _flags = flags;
    }

    protected override void RenderCall()
    {
        if (_type.Render(_label, _format, _flags)) _type.Invoke();
    }
}

public partial class Window
{
    public Window AddSlider(string label, ISlider type, string format = "",
        ImGuiSliderFlags flags = ImGuiSliderFlags.None)
    {
        RegisterWidget(new Slider(label, type, format, flags));
        return this;
    }
}