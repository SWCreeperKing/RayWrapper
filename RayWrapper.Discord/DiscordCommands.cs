using RayWrapper.GameConsole;
using static RayWrapper.Discord.DiscordIntegration;
using static RayWrapper.GameBox;
using static RayWrapper.GameConsole.GameConsole;

namespace RayWrapper.Discord;

public class DiscordCommands : ICommandModule
{
    
    [Command("discord"), Help("discord rich presence status/recheck")]
    public static string DiscordCheck(string[] args)
    {
        if (DiscordIntegration.discordAppId == string.Empty) return $"Discord Integration is not set up for [{AppName}]";
        if (discordAlive) return $"[{AppName}] is Connected to discord!";
        WriteToConsole("Disconnected from discord, attempting to connect");
        CheckDiscord(DiscordIntegration.discordAppId);
        return discordAlive ? "Reconnected!" : "Failed, Try again later!";
    }
}