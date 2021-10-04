using System;

namespace RayWrapper.GameConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string name;
        public CommandAttribute(string name) => this.name = name.ToLower();
    }
}