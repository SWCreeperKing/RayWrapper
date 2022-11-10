using System.Reflection;
using static RayWrapper.LegacyUI.GameConsole.CommandLineColor;

namespace RayWrapper.LegacyUI.GameConsole;

public static class CommandRegister
{
    public static readonly Dictionary<string, MethodInfo> Commands = new();
    public static readonly Dictionary<string, string> Aliases = new();
    public static readonly Dictionary<string, string> Definitions = new();

    public static void RegisterCommand(Type t)
    {
        if (t.GetInterfaces().Contains(typeof(ICommandModule)))
        {
            foreach (var method in t.GetMethods())
            {
                var commandName = string.Empty;
                foreach (var att in method.GetCustomAttributes())
                {
                    switch (att)
                    {
                        case CommandAttribute ca:
                            if (commandName != string.Empty) continue;
                            if (Commands.ContainsKey(ca.name))
                                Console.WriteLine($"[WARN] [{ca.name}] is already registered");
                            else if (Commands.ContainsValue(method))
                                Console.WriteLine($"[WARN] [{method.Name}] is already registered");
                            else Commands.Add(commandName = ca.name, method);
                            break;
                        case AliasAttribute aa:
                            if (commandName == string.Empty) continue;
                            foreach (var a in aa.aliases)
                                if (Aliases.ContainsKey(a))
                                    Console.WriteLine(
                                        $"[WARN] [{a}] could not be listed as an alias for {commandName} because alias already exists for another command");
                                else Aliases.Add(a, commandName);
                            break;
                        case HelpAttribute ha:
                            if (commandName == string.Empty) continue;
                            if (Definitions.ContainsKey(commandName))
                                Console.WriteLine($"[WARN] help definition for [{commandName}] already listed");
                            else Definitions.Add(commandName, ha.definition);
                            break;
                    }
                }
            }
        }
        else Console.WriteLine($"[ERR] [{t.Name}] is not of ICommandModule");
    }

    public static void RegisterCommand<T>() where T : ICommandModule => RegisterCommand(typeof(T));

    public static string[] ExecuteCommand(string cmd, params string[] args)
    {
        string[] arr;
        if (Commands.ContainsKey(cmd)) arr = ExecuteCommand(Commands[cmd], cmd, args);
        else if (Aliases.ContainsKey(cmd)) arr = ExecuteCommand(Commands[Aliases[cmd]], cmd, args);
        else arr = new[] { $"{RED}Invalid Command: [{cmd}]" };
        return arr;
    }

    private static string[] ExecuteCommand(MethodInfo command, string cmd, params string[] args)
    {
        switch (command.ReturnType.Name.ToLower())
        {
            case "void":
                command.Invoke(null, new[] { args });
                return new[] { $"Executed [{cmd}]" };
            case "string":
                var r = command.Invoke(null, new[] { args });
                return new[] { (string)r };
            case "string[]":
                return (string[])command.Invoke(null, new[] { args });
        }

        return new[] { $"{DARKRED}Failed to execute [{cmd}]" };
    }
}