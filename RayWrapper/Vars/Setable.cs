namespace RayWrapper.Vars
{
    public abstract class Setable<T>
    {
        protected abstract T GetThis();

        public void Set(T overrider)
        {
            foreach (var field in GetThis().GetType().GetFields()) field.SetValue(GetThis(), field.GetValue(overrider));
        }
    }
}