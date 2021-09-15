using System;

namespace RayWrapper.Vars
{
    public class Actionable<T>
    {
        public T val;
        public Func<T> valFunc;

        public Actionable(T val, Func<T> valFunc = null) => (this.val, this.valFunc) = (val, valFunc);
        public Actionable(Func<T> valFunc = null) => this.valFunc = valFunc;
        public static implicit operator T(Actionable<T> t) => t.valFunc is null ? t.val : t.valFunc.Invoke() ?? t.val;
        public static implicit operator Actionable<T>(T t) => new(t);
        public static implicit operator Actionable<T>(Func<T> t) => new(t);
    }
}

