using System;

namespace RayWrapper.Vars;

/// <summary>
/// this allows for a type to be of either static (variable) or dynamic (function).
/// </summary>
public class Actionable<T>
{
    public T val;
    public Func<T> valFunc; // can't save if this is used

    public Actionable()
    {
        // json constructor
        val = default;
    }
        
    public Actionable(T val, Func<T> valFunc = null) => (this.val, this.valFunc) = (val, valFunc);
    public Actionable(Func<T> valFunc = null) => this.valFunc = valFunc;
        
    public static implicit operator T(Actionable<T> t) => t.valFunc is null ? t.val : t.valFunc.Invoke() ?? t.val;
    public static implicit operator Actionable<T>(T t) => new(t);
    public static implicit operator Actionable<T>(Func<T> t) => new(t);
    public override string ToString() => (valFunc is null ? val : valFunc.Invoke() ?? val).ToString();
    public Actionable<T> Copy() => new() { val = val, valFunc = valFunc };
}