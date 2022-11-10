namespace RayWrapper.LegacyUI.GameConsole;

[AttributeUsage(AttributeTargets.Method)]
public class AliasAttribute : Attribute
{
    public string[] aliases;
    public AliasAttribute(params string[] aliases) => this.aliases = aliases.Select(a => a.ToLower()).ToArray();
}