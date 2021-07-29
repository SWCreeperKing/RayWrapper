using System;
using System.Reflection;

namespace RayWrapper.Vars
{
    public abstract class Setable<T>
    {
        protected abstract T GetThis();

        public void Set(T overrider)
        {
            foreach (var field in GetThis().GetType().GetFields())
            {
                try
                {
                    field.SetValue(GetThis(), field.GetValue(overrider));
                }
                catch (TargetException e)
                {
                    Console.WriteLine($"FIELD: {field.Name} CORRUPT? {e.Message}");
                }
            }
        }
    }
}