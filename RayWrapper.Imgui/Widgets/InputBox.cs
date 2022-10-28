using System.Numerics;
using System.Text;
using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class InputBox : Widget
{
    public enum InputType
    {
        String,
        StringMulti,
        StringHint,
        Int,
        Double,
        Float,
        Vector2,
        Vector3,
        Vector4,
    }

    private uint maxLength = 512;
    private ImGuiInputTextFlags _flags;
    private string _label;
    private InputType _type;
    private Vector2 _size = Vector2.Zero;
    private string _hint;

    // data
    private string _format;
    private string _inString;
    private byte[] _inByteString = new byte[512];
    private int _inInt;
    private double _inDouble;
    private float _inFloat;
    private Vector2 _inV2;
    private Vector3 _inV3;
    private Vector4 _inV4;
    private Action<string> _outString;
    private Action<int> _outInt;
    private Action<double> _outDouble;
    private Action<float> _outFloat;
    private Action<Vector2> _outV2;
    private Action<Vector3> _outV3;
    private Action<Vector4> _outV4;

    public InputBox(string label, string inString, Action<string> action, uint maxLength = 512,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        SetLabelFlagAndType(label, InputType.String, flags);
        _inByteString = Encoding.ASCII.GetBytes(_inString = inString);
        this.maxLength = (uint) Math.Max(maxLength, _inByteString.Length);
        Array.Resize(ref _inByteString, (int) this.maxLength);
        _outString = action;
    }

    public InputBox(string label, string inString, Vector2 size, Action<string> action, uint maxLength = 512,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        SetLabelFlagAndType(label, InputType.StringMulti, flags);
        _size = size;
        this.maxLength = maxLength;
        _inString = inString;
        _outString = action;
    }

    public InputBox(string label, string inString, string hint, Action<string> action, uint maxLength = 512,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        SetLabelFlagAndType(label, InputType.StringHint, flags);
        _hint = hint;
        this.maxLength = maxLength;
        _inString = inString;
        _outString = action;
    }

    public InputBox(string label, int inInt, Action<int> action, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        SetLabelFlagAndType(label, InputType.Int, flags);
        _inInt = inInt;
        _outInt = action;
    }

    public InputBox(string label, double inDouble, Action<double> action, string format,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        SetLabelFlagAndType(label, InputType.Double, flags);
        _format = format;
        _inDouble = inDouble;
        _outDouble = action;
    }

    public InputBox(string label, float inFloat, Action<float> action, string format,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        SetLabelFlagAndType(label, InputType.Float, flags);
        _format = format;
        _inFloat = inFloat;
        _outFloat = action;
    }

    public InputBox(string label, Vector2 inV2, Action<Vector2> action, string format,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        SetLabelFlagAndType(label, InputType.Vector2, flags);
        _format = format;
        _inV2 = inV2;
        _outV2 = action;
    }

    public InputBox(string label, Vector3 inV3, Action<Vector3> action, string format,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        SetLabelFlagAndType(label, InputType.Vector3, flags);
        _format = format;
        _inV3 = inV3;
        _outV3 = action;
    }

    public InputBox(string label, Vector4 inV4, Action<Vector4> action, string format,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        SetLabelFlagAndType(label, InputType.Vector4, flags);
        _format = format;
        _inV4 = inV4;
        _outV4 = action;
    }

    protected override void RenderCall()
    {
        if (InputCheck()) Action();
    }

    private bool InputCheck() =>
        _type switch
        {
            InputType.String => ImGui.InputText(_label, _inByteString, maxLength, _flags),
            InputType.StringMulti => ImGui.InputTextMultiline(_label, ref _inString, maxLength, _size,
                _flags),
            InputType.StringHint => ImGui.InputTextWithHint(_label, _hint, ref _inString, maxLength,
                _flags),
            InputType.Int => ImGui.InputInt(_label, ref _inInt, 1, 5, _flags),
            InputType.Double => ImGui.InputDouble(_label, ref _inDouble, 1, 5, _format, _flags),
            InputType.Float => ImGui.InputFloat(_label, ref _inFloat, 1, 5, _format, _flags),
            InputType.Vector2 => ImGui.InputFloat2(_label, ref _inV2, _format, _flags),
            InputType.Vector3 => ImGui.InputFloat3(_label, ref _inV3, _format, _flags),
            InputType.Vector4 => ImGui.InputFloat4(_label, ref _inV4, _format, _flags),
            _ => throw new ArgumentOutOfRangeException()
        };

    private void Action()
    {
        switch (_type)
        {
            case InputType.String:
                _outString?.Invoke(System.Text.Encoding.UTF8.GetString(_inByteString).TrimEnd('\0'));
                break;
            case InputType.StringMulti:
                _outString?.Invoke(_inString);
                break;
            case InputType.StringHint:
                _outString?.Invoke(_inString);
                break;
            case InputType.Int:
                _outInt?.Invoke(_inInt);
                break;
            case InputType.Double:
                _outDouble?.Invoke(_inDouble);
                break;
            case InputType.Float:
                _outFloat?.Invoke(_inFloat);
                break;
            case InputType.Vector2:
                _outV2?.Invoke(_inV2);
                break;
            case InputType.Vector3:
                _outV3?.Invoke(_inV3);
                break;
            case InputType.Vector4:
                _outV4?.Invoke(_inV4);
                break;
        }
    }

    private void SetLabelFlagAndType(string label, InputType type, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        (_label, _flags, _type) = (label, flags, type);
    }
}

public partial class Window
{
    public Window AddInputBox(string label, string inString, Action<string> action, uint maxLength = 512,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        RegisterWidget(new InputBox(label, inString, action, maxLength, flags));
        return this;
    }

    public Window AddInputBox(string label, string inString, Vector2 size, Action<string> action, uint maxLength = 512,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        RegisterWidget(new InputBox(label, inString, size, action, maxLength, flags));
        return this;
    }

    public Window AddInputBox(string label, string inString, string hint, Action<string> action, uint maxLength = 512,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        RegisterWidget(new InputBox(label, inString, hint, action, maxLength, flags));
        return this;
    }

    public Window AddInputBox(string label, int inInt, Action<int> action,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        RegisterWidget(new InputBox(label, inInt, action, flags));
        return this;
    }

    public Window AddInputBox(string label, double inDouble, Action<double> action, string format,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        RegisterWidget(new InputBox(label, inDouble, action, format));
        return this;
    }

    public Window AddInputBox(string label, float inFloat, Action<float> action, string format,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        RegisterWidget(new InputBox(label, inFloat, action, format, flags));
        return this;
    }

    public Window AddInputBox(string label, Vector2 inV2, Action<Vector2> action, string format,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        RegisterWidget(new InputBox(label, inV2, action, format, flags));
        return this;
    }

    public Window AddInputBox(string label, Vector3 inV3, Action<Vector3> action, string format,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        RegisterWidget(new InputBox(label, inV3, action, format, flags));
        return this;
    }

    public Window AddInputBox(string label, Vector4 inV4, Action<Vector4> action, string format,
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        RegisterWidget(new InputBox(label, inV4, action, format, flags));
        return this;
    }
}