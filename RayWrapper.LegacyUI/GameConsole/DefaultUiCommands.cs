using RayWrapper.Base.CommandRegister;
using static RayWrapper.LegacyUI.GameConsole.GameConsole;

namespace RayWrapper.LegacyUI.GameConsole;

public class DefaultUiCommands : ICommandModule
{
    [Command("clear"), Help("clear (true/false)\nclears screen, true will not display message")]
    public static string Clear(string[] args)
    {
        var c = ClearOutput();
        if (args.Length <= 0 || args[0].ToLower()[0] is not 't' or '1')
            return $"{CommandLineColor.BLUE}Cleared {c} lines";
        return string.Empty;
    }
}