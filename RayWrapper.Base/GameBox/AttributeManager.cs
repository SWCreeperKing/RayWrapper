using System.Reflection;
using static RayWrapper.Base.GameBox.Logger.Level;

namespace RayWrapper.Base.GameBox;

public static class AttributeManager
{
    public enum PlacerType
    {
        BeforeInit,
        AfterInit,
        BeforeUpdate,
        AfterUpdate,
        BeforeRender,
        AfterRender,
        BeforeDispose,
        AfterDispose
    }

    public static readonly Dictionary<PlacerType, List<MethodInfo>> ExtraRunners = new();

    public static void Init()
    {
        List<MethodInfo> methods = new();
        var types =  GetAssemblies().Select(a => a.GetTypes()
            .Select(t => t.GetMethods().Where(m => m.GetCustomAttributes<GameBoxWedgeAttribute>().Any())));
        foreach (var typeLists in types)
        foreach (var methodLists in typeLists)
        {
            methods.AddRange(methodLists);
        }

        var amount = methods.Count;
        if (amount <= 0)
        {
            Logger.Log(Warning, "No Types Registered");
            return;
        }

        methods.ForEach(AddMethodInfo);

        foreach (var k in ExtraRunners.Keys)
        {
            Logger.Log(Info, $"Organizing Placer: [{k}]");
            ExtraRunners[k] = ExtraRunners[k].OrderBy(mi =>
            {
                var att = mi.GetCustomAttributes<GameBoxWedgeAttribute>();
                return !att.Any() ? int.MaxValue : att.First().priority;
            }).ToList();
        }

        Logger.Log(Info, $"Registered [{amount}] extra loops");
    }

    public static IEnumerable<Assembly> GetAssemblies()
    {
        var returnAssemblies = new List<Assembly>();
        var loadedAssemblies = new HashSet<string>();
        var assembliesToCheck = new Queue<Assembly>();

        assembliesToCheck.Enqueue(Assembly.GetEntryAssembly()!);

        while (assembliesToCheck.Any())
        {
            var assemblyToCheck = assembliesToCheck.Dequeue();

            foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
            {
                if (loadedAssemblies.Contains(reference.FullName)) continue;
                var assembly = Assembly.Load(reference);
                assembliesToCheck.Enqueue(assembly);
                loadedAssemblies.Add(reference.FullName);
                returnAssemblies.Add(assembly);
            }
        }

        return returnAssemblies;
    }

    private static void AddMethodInfo(MethodInfo mi)
    {
        var att = mi.GetCustomAttributes<GameBoxWedgeAttribute>();
        if (!att.Any()) return;
        var type = att.First().type;
        if (!ExtraRunners.ContainsKey(type)) ExtraRunners.Add(type, new List<MethodInfo>());
        ExtraRunners[type].Add(mi);
        Logger.Log(Debug, $"Registered method: [{mi.Name}] in [{type}]");
    }

    public static void ExtraRunner(PlacerType type, float dt = 0)
    {
        if (!ExtraRunners.ContainsKey(type)) return;
        if (type is PlacerType.BeforeUpdate or PlacerType.AfterUpdate)
        {
            ExtraRunners[type].ForEach(m => InvokeMethod(type, m, parameters: new object[] { dt }));
        }
        else ExtraRunners[type].ForEach(m => InvokeMethod(type, m));
    }

    private static void InvokeMethod(PlacerType type, MethodInfo mi, object? obj = null, object?[]? parameters = null)
    {
        try
        {
            mi.Invoke(obj, parameters);
        }
        catch (Exception e)
        {
            Logger.Log(e, $"Threw from type: [{type}] and method [{mi.Name}]");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GameBoxWedgeAttribute : Attribute
    {
        public PlacerType type;
        public int priority;

        public GameBoxWedgeAttribute(PlacerType type, int priority = 99999)
        {
            this.type = type;
            this.priority = priority;
        }
    }
}