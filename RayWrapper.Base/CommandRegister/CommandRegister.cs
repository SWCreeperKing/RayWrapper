﻿using System.Reflection;
using RayWrapper.Base.GameBox;
using static RayWrapper.Base.GameBox.Logger.Level;

namespace RayWrapper.Base.CommandRegister;

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
                            {
                                Logger.Log(Warning, $"Command Register: [{ca.name}] is already registered");
                            }
                            else if (Commands.ContainsValue(method))
                            {
                                Logger.Log(Warning, $"Command Register: [{method.Name}] is already registered");
                            }
                            else Commands.Add(commandName = ca.name, method);

                            break;
                        case AliasAttribute aa:
                            if (commandName == string.Empty) continue;
                            foreach (var a in aa.aliases)
                                if (Aliases.ContainsKey(a))
                                {
                                    Logger.Log(Warning,
                                        $"Command Register: [{a}] could not be listed as an alias for {commandName} because alias already exists for another command");
                                }
                                else Aliases.Add(a, commandName);

                            break;
                        case HelpAttribute ha:
                            if (commandName == string.Empty) continue;
                            if (Definitions.ContainsKey(commandName))
                            {
                                Logger.Log(Warning,
                                    $"Command Register: help definition for [{commandName}] already listed");
                            }
                            else Definitions.Add(commandName, ha.definition);

                            break;
                    }
                }
            }
        }
        else Logger.Log(Error, "Command Register: [{t.Name}] is not of ICommandModule");
    }

    public static void RegisterCommand<T>() where T : ICommandModule => RegisterCommand(typeof(T));

    public static (Logger.Level, string)[] ExecuteCommand(string cmd, params string[] args)
    {
        Logger.Log($"From Console: (command in)[{cmd}..{string.Join(" ", args)}]");
        (Logger.Level, string)[] arr;
        if (Commands.ContainsKey(cmd)) arr = ExecuteCommand(Commands[cmd], cmd, args);
        else if (Aliases.ContainsKey(cmd)) arr = ExecuteCommand(Commands[Aliases[cmd]], cmd, args);
        else arr = new[] { (Warning, $"Invalid Command: [{cmd}]") };
        return arr;
    }

    private static (Logger.Level, string)[] ExecuteCommand(MethodInfo command, string cmd, params string[] args)
    {
        switch (command.ReturnType.Name.ToLower())
        {
            case "void":
                command.Invoke(null, new[] { args });
                return new[] { (Info, $"Executed [{cmd}]") };
            case "valuetuple`2":
                var r = command.Invoke(null, new[] { args });
                return new[] { (ValueTuple<Logger.Level, string>) r };
            case "valuetuple`2[]":
                return ((Logger.Level, string)[]) command.Invoke(null, new[] { args });
        }

        return new[] { (SoftError, $"Failed to execute [{cmd}]") };
    }
}