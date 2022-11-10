using System.Numerics;
using System.Text;
using ImGuiNET;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Imgui.Widgets;

public interface IInputType
{
    public bool Render(string label, ImGuiInputTextFlags flags);
    public void Invoke();
}

public abstract class InputTypeBase<T> : IInputType
{
    protected T inVar;
    protected Action<T> outVar;

    protected InputTypeBase(T inVar, Action<T> outVar)
    {
        this.inVar = inVar;
        this.outVar = outVar;
    }

    public abstract bool Render(string label, ImGuiInputTextFlags flags);
    public virtual void Invoke() => outVar?.Invoke(inVar);
}

public class InputInts : InputTypeBase<int[]>
{
    public InputInts(int[] inVar, Action<int[]> outVar) : base(inVar, outVar)
    {
        if (inVar.Length is < 2 or > 4) throw new ArgumentException("InputInts array must be of length: 2, 3, or 4");
    }

    public override bool Render(string label, ImGuiInputTextFlags flags)
    {
        return inVar.Length switch
        {
            2 => ImGui.InputInt2(label, ref inVar[0], flags),
            3 => ImGui.InputInt3(label, ref inVar[0], flags),
            4 => ImGui.InputInt4(label, ref inVar[0], flags)
        };
    }
}

public class InputString : InputTypeBase<string>
{
    private byte[] _rInVar;
    private uint _maxLength;

    public InputString(string inVar, Action<string> outVar, uint maxLength = 512) : base(inVar, outVar)
    {
        _rInVar = Encoding.ASCII.GetBytes(inVar);
        _maxLength = (uint) Math.Max(maxLength, _rInVar.Length);
        Array.Resize(ref _rInVar, (int) _maxLength);
    }

    public override bool Render(string label, ImGuiInputTextFlags flags)
    {
        return ImGui.InputText(label, _rInVar, _maxLength, flags);
    }

    public override void Invoke() => outVar?.Invoke(Encoding.UTF8.GetString(_rInVar).TrimEnd('\0'));
}

public class InputHint : InputTypeBase<string>
{
    private string _hint;
    private uint _maxLength;

    public InputHint(string inVar, Action<string> outVar, string hint, uint maxLength = 512) : base(inVar, outVar)
    {
        _hint = hint;
        _maxLength = (uint) Math.Max(inVar.Length, maxLength);
    }

    public override bool Render(string label, ImGuiInputTextFlags flags)
    {
        return ImGui.InputTextWithHint(label, _hint, ref inVar, _maxLength, flags);
    }
}

public class InputMulti : InputTypeBase<string>
{
    private Vector2 _size;
    private uint _maxLength;

    public InputMulti(string inVar, Action<string> outVar, Vector2 size, uint maxLength = 512) : base(inVar, outVar)
    {
        _size = size;
        _maxLength = (uint) Math.Max(inVar.Length, maxLength);
    }

    public override bool Render(string label, ImGuiInputTextFlags flags)
    {
        return ImGui.InputTextMultiline(label, ref inVar, _maxLength, _size, flags);
    }
}

public abstract class NumberInputType<T> : InputTypeBase<T>
{
    protected T step;
    protected T fastStep;
    protected string format;

    public NumberInputType(T inVar, Action<T> outVar, string format, T step, T fastStep) : base(inVar, outVar)
    {
        this.step = step;
        this.fastStep = fastStep;
        this.format = format;
    }
}

public class InputInt : NumberInputType<int>
{
    public InputInt(int inVar, Action<int> outVar, int step = 1, int fastStep = 5) : base(inVar, outVar, "", step,
        fastStep)
    {
    }

    public override bool Render(string label, ImGuiInputTextFlags flags)
    {
        return ImGui.InputInt(label, ref inVar, step, fastStep, flags);
    }
}

public class InputFloat : NumberInputType<float>
{
    public InputFloat(float inVar, Action<float> outVar, string format = "", float step = 1, float fastStep = 5) : base(
        inVar, outVar, format, step, fastStep)
    {
    }

    public override bool Render(string label, ImGuiInputTextFlags flags)
    {
        return ImGui.InputFloat(label, ref inVar, step, fastStep, format, flags);
    }
}

public class InputDouble : NumberInputType<double>
{
    public InputDouble(double inVar, Action<double> outVar, string format = "", double step = 1,
        double fastStep = 5) : base(
        inVar, outVar, format, step, fastStep)
    {
    }

    public override bool Render(string label, ImGuiInputTextFlags flags)
    {
        return ImGui.InputDouble(label, ref inVar, step, fastStep, format, flags);
    }
}

public abstract class VectorInputType<T> : InputTypeBase<T>
{
    protected string format;

    protected VectorInputType(T inVar, Action<T> outVar, string format = "") : base(inVar, outVar)
    {
        this.format = format;
    }
}

public class InputVector2 : VectorInputType<Vector2>
{
    public InputVector2(Vector2 inVar, Action<Vector2> outVar, string format = "") : base(inVar, outVar, format)
    {
    }

    public override bool Render(string label, ImGuiInputTextFlags flags)
    {
        return ImGui.InputFloat2(label, ref inVar, format, flags);
    }
}

public class InputVector3 : VectorInputType<Vector3>
{
    public InputVector3(Vector3 inVar, Action<Vector3> outVar, string format = "") : base(inVar, outVar, format)
    {
    }

    public override bool Render(string label, ImGuiInputTextFlags flags)
    {
        return ImGui.InputFloat3(label, ref inVar, format, flags);
    }
}

public class InputVector4 : VectorInputType<Vector4>
{
    public InputVector4(Vector4 inVar, Action<Vector4> outVar, string format = "") : base(inVar, outVar, format)
    {
    }

    public override bool Render(string label, ImGuiInputTextFlags flags)
    {
        return ImGui.InputFloat4(label, ref inVar, format, flags);
    }
}

public class InputBox : GameObject
{
    private ImGuiInputTextFlags _flags;
    private string _label;
    private IInputType _type;

    public InputBox(string label, IInputType inputType, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        _label = label;
        _type = inputType;
        _flags = flags;
    }

    protected override void RenderCall()
    {
        if (_type.Render(_label, _flags)) _type.Invoke();
    }
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddInputBox(string label, IInputType inputType, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        RegisterGameObj(new InputBox(label, inputType, flags));
        return this;
    }
}