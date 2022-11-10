using System.Numerics;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Imgui.Widgets;

public class Button : GameObject
{
    private enum ButtonType
    {
        Normal,
        Image,
        Arrow,
        Small,
        Radio,
        Color
    }

    public string label;
    public Action clicked;

    private ButtonType _type;
    private ImGuiColorEditFlags _colorFlags;
    private ImGuiDir _dir;
    private bool _isActive;
    private Vector4 _color;
    private Vector2 _maxSize = Vector2.Zero;

    //img button
    private nint _texture;
    private Vector2? _uv0, _uv1;
    private int _framePadding = int.MinValue;
    private Vector4? _backgroundColor, _tint;

    public Button(string label, Action clicked, Vector2? maxSize = null)
    {
        this.label = label;
        this.clicked = clicked;
        _maxSize = maxSize ?? Vector2.Zero;
        _type = maxSize is null ? ButtonType.Small : ButtonType.Normal;
    }

    public Button(string labelId, Action clicked, ImGuiDir direction)
    {
        label = labelId;
        this.clicked = clicked;
        _dir = direction;
        _type = ButtonType.Arrow;
    }

    public Button(string label, Action clicked, bool startActive)
    {
        this.label = label;
        this.clicked = clicked;
        _isActive = startActive;
        _type = ButtonType.Radio;
    }

    public Button(string descId, Action clicked, Color color,
        ImGuiColorEditFlags flags = ImGuiColorEditFlags.DefaultOptions,
        Vector2? maxSize = null)
    {
        label = descId;
        this.clicked = clicked;
        _color = color.ToV4();
        _colorFlags = flags;
        _maxSize = maxSize ?? Vector2.Zero;
        _type = ButtonType.Color;
    }

    public Button(nint texture, Action clicked, Vector2? maxSize = null, Vector2? uv0 = null, Vector2? uv1 = null,
        int framePadding = int.MinValue, Color? backgroundColor = null, Color? tint = null)
    {
        _texture = texture;
        this.clicked = clicked;
        _maxSize = maxSize ?? Vector2.Zero;
        _uv0 = uv0;
        _uv1 = uv1;
        _framePadding = framePadding;
        _backgroundColor = backgroundColor?.ToV4();
        _tint = tint?.ToV4();
        _type = ButtonType.Image;
    }

    protected override void RenderCall()
    {
        if (CheckButton()) clicked();
    }

    public bool CheckButton() =>
        _type switch
        {
            ButtonType.Normal => ImGui.Button(label, _maxSize),
            ButtonType.Image => ImageButton(),
            ButtonType.Arrow => ImGui.ArrowButton(label, _dir),
            ButtonType.Small => ImGui.SmallButton(label),
            ButtonType.Radio => RadioButton(),
            ButtonType.Color => ImGui.ColorButton(label, _color, _colorFlags, _maxSize),
            _ => throw new ArgumentOutOfRangeException()
        };

    private bool RadioButton()
    {
        if (ImGui.RadioButton(label, _isActive)) _isActive = !_isActive;
        return _isActive;
    }

    // idk how the overloading methods act :p
    private bool ImageButton()
    {
        if (_uv0 is null) return ImGui.ImageButton(_texture, _maxSize);
        if (_uv1 is null) return ImGui.ImageButton(_texture, _maxSize, _uv0.Value);
        if (_framePadding == int.MinValue) return ImGui.ImageButton(_texture, _maxSize, _uv0.Value, _uv1.Value);
        if (_backgroundColor is null)
        {
            return ImGui.ImageButton(_texture, _maxSize, _uv0.Value, _uv1.Value, _framePadding);
        }

        if (_tint is null)
        {
            return ImGui.ImageButton(_texture, _maxSize, _uv0.Value, _uv1.Value, _framePadding, _backgroundColor.Value);
        }

        return ImGui.ImageButton(_texture, _maxSize, _uv0.Value, _uv1.Value, _framePadding, _backgroundColor.Value,
            _tint.Value);
    }
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddButton(string label, Action clicked, Vector2? maxSize = null)
    {
        RegisterGameObj(new Button(label, clicked, maxSize));
        return this;
    }

    public CompoundWidgetBuilder AddButton(string label, Action clicked, ImGuiDir direction)
    {
        RegisterGameObj(new Button(label, clicked, direction));
        return this;
    }

    public CompoundWidgetBuilder AddButton(string label, Action clicked, bool startActive)
    {
        RegisterGameObj(new Button(label, clicked, startActive));
        return this;
    }

    public CompoundWidgetBuilder AddButton(string label, Action clicked, Color color,
        ImGuiColorEditFlags flags = ImGuiColorEditFlags.DefaultOptions, Vector2? maxSize = null)
    {
        RegisterGameObj(new Button(label, clicked, color, flags, maxSize));
        return this;
    }

    public CompoundWidgetBuilder AddButton(nint texture, Action clicked, Vector2? maxSize = null, Vector2? uv0 = null,
        Vector2? uv1 = null, int framePadding = int.MinValue, Color? backgroundColor = null, Color? tint = null)
    {
        RegisterGameObj(new Button(texture, clicked, maxSize, uv0, uv1, framePadding, backgroundColor, tint));
        return this;
    }
}