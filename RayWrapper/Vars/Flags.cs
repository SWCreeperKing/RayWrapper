using System;
using System.Collections.Generic;

namespace RayWrapper.Vars
{
    public class Flags<T>
    {
        public IDictionary<string, T> flags = new Dictionary<string, T>();

        public T GetFlag(Enum e, T def = default)
        {
            if (flags.ContainsKey(e.ToString())) return flags[e.ToString()];
            return flags[e.ToString()] = def;
        }

        public void SetFlag(Enum e, T t) => flags[e.ToString()] = t;

        public T this[Enum e]
        {
            get => GetFlag(e);
            set => SetFlag(e, value);
        }

        public T this[params Enum[] ee]
        {
            set
            {
                foreach (var e in ee) SetFlag(e, value);
            }
        }
    }
}