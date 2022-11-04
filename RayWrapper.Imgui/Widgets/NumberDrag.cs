using System.Numerics;
using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public interface INumberDrag
{
    public bool Render(string label, string format, ImGuiSliderFlags flags);
    public void Invoke();
}

public abstract class NumberDragBase<TVar, TMinMax> : INumberDrag
{
    protected TVar inVar;
    protected TMinMax speed;
    protected TMinMax min;
    protected TMinMax max;
    protected Action<TVar> action;

    protected NumberDragBase(TVar inVar, Action<TVar> action, TMinMax speed, TMinMax min, TMinMax max)
    {
        this.inVar = inVar;
        this.speed = speed;
        this.min = min;
        this.max = max;
        this.action = action;
    }

    public abstract bool Render(string label, string format, ImGuiSliderFlags flags);
    public virtual void Invoke() => action?.Invoke(inVar);
}

public class DragInt : NumberDragBase<int, int>
{
    public DragInt(int inVar, Action<int> action, int speed = 1, int min = -500, int max = 500) : base(inVar, action,
        speed, min, max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.DragInt(label, ref inVar, speed, min, max, format, flags);
    }
}

public class DragInts : NumberDragBase<int[], int>
{
    public DragInts(int[] inVar, Action<int[]> action, int speed = 1, int min = -500, int max = 500) : base(inVar,
        action, speed, min, max)
    {
        if (inVar.Length is < 2 or > 4) throw new ArgumentException("DragInts array must be of length: 2, 3, or 4");
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return inVar.Length switch
        {
            2 => ImGui.DragInt2(label, ref inVar[0], speed, min, max, format, flags),
            3 => ImGui.DragInt3(label, ref inVar[0], speed, min, max, format, flags),
            4 => ImGui.DragInt4(label, ref inVar[0], speed, min, max, format, flags),
        };
    }
}

public class DragFloat : NumberDragBase<float, float>
{
    public DragFloat(float inVar, Action<float> action, float speed = 1, float min = -500, float max = 500) : base(
        inVar, action, speed, min, max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.DragFloat(label, ref inVar, speed, min, max, format, flags);
    }
}

public class DragVector2 : NumberDragBase<Vector2, float>
{
    public DragVector2(Vector2 inVar, Action<Vector2> action, float speed = 1, float min = -500, float max = 500) :
        base(inVar, action, speed, min, max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.DragFloat2(label, ref inVar, speed, min, max, format, flags);
    }
}

public class DragVector3 : NumberDragBase<Vector3, float>
{
    public DragVector3(Vector3 inVar, Action<Vector3> action, float speed = 1, float min = -500, float max = 500) :
        base(inVar, action, speed, min, max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.DragFloat3(label, ref inVar, speed, min, max, format, flags);
    }
}

public class DragVector4 : NumberDragBase<Vector4, float>
{
    public DragVector4(Vector4 inVar, Action<Vector4> action, float speed = 1, float min = -500, float max = 500) :
        base(inVar, action, speed, min, max)
    {
    }

    public override bool Render(string label, string format, ImGuiSliderFlags flags)
    {
        return ImGui.DragFloat4(label, ref inVar, speed, min, max, format, flags);
    }
}

public class NumberDrag : Widget
{
    public string label;
    public string format;
    public INumberDrag numberDrag;
    public ImGuiSliderFlags flags;

    public NumberDrag(string label, INumberDrag numberDrag, string format = "",
        ImGuiSliderFlags flags = ImGuiSliderFlags.None)
    {
        this.label = label;
        this.format = format;
        this.numberDrag = numberDrag;
        this.flags = flags;
    }

    protected override void RenderCall()
    {
        if (numberDrag.Render(label, format, flags)) numberDrag.Invoke();
    }
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddNumberDrag(string label, INumberDrag numberDrag, string format = "",
        ImGuiSliderFlags flags = ImGuiSliderFlags.None)
    {
        RegisterWidget(new NumberDrag(label, numberDrag, format, flags));
        return this;
    }
}