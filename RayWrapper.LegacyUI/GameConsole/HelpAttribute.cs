namespace RayWrapper.LegacyUI.GameConsole;

[AttributeUsage(AttributeTargets.Method)]
public class HelpAttribute : Attribute
{
    public string definition;
    public HelpAttribute(string definition) => this.definition = $"\t{definition}".Replace("\n", "\n\t");
}