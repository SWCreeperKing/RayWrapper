namespace RayWrapper.Base.CommandRegister;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string name;
    public CommandAttribute(string name) => this.name = name.ToLower();
}