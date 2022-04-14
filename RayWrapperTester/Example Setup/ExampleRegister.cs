using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RayWrapperTester.Example_Setup;

public static class ExampleRegister
{
    public static Dictionary<string, Example> Examples;

    public static void Init()
    {
        var asm = Assembly.GetExecutingAssembly();
        var types = asm.GetTypes();
        var typesWithCustomAtt = types.Where(t => t.GetCustomAttributes<ExampleAttribute>().Any());
        var typesAndCustomAtt =
            typesWithCustomAtt.Select(t => (type: t, att: t.GetCustomAttribute<ExampleAttribute>()));

        Examples = typesAndCustomAtt.ToDictionary(t => t.att!.tabName,
            t => (Example) Activator.CreateInstance(t.type, t.att?.tabName));
    }
}