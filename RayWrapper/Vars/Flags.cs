using System;
using System.Collections.Generic;
using System.Linq;

namespace RayWrapper.Vars
{
    public class Flags<T>
    {
        public IDictionary<string, T> flags = new Dictionary<string, T>();

        public string StringToHex(string text) => string.Join("", text.Select(c => ((int)c).ToString("X2")));

        public T GetFlag(string hex, T def = default)
        {
            if (flags.ContainsKey(hex)) return flags[hex];
            return flags[hex] = def;
        }

        public void SetFlag(string hex, T t) => flags[hex] = t; 
        
        public T this[Enum e]
        {
            get => GetFlag(StringToHex($"{e}"));
            set => SetFlag(StringToHex($"{e}"), value);
        }

        public T this[params Enum[] ee]
        {
            set
            {
                foreach (var e in ee) SetFlag(StringToHex($"{e}"), value);
            }
        }
    }
}