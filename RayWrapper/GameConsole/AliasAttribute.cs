using System;
using System.Linq;

namespace RayWrapper.GameConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AliasAttribute : Attribute
    {
        public string[] aliases;
        public AliasAttribute(params string[] aliases) => this.aliases = aliases.Select(a => a.ToLower()).ToArray();
    }
}