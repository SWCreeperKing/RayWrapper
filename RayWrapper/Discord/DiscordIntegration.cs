using System;
using DiscordRPC;
using RayWrapper.Vars;
using static RayWrapper.GameConsole.CommandLineColor;
using static RayWrapper.GameConsole.GameConsole;
using static RayWrapper.Vars.Logger.Level;

namespace RayWrapper.Discord;

/// <summary>
/// the following is recommended to put in an the <see cref="GameLoop.Init()"/> method of the <see cref="GameLoop"/>
/// <list type="numer">
/// <listheader>
/// How to use discord integration:
/// </listheader>
/// <item>1. create an application on <a href="https://discord.com/developers/applications">the dev portal</a></item>
/// <item>2. set <see cref="GameBox.discordAppId"/> to the Application Id of the rich presence</item>
/// <item>3. call <see cref="GameBox.InitDiscord()"/></item>
/// </list>
/// the 6 <see cref="Func{TResult}"/>s are additional fields for rich presence.
/// here are the 6 fields and what they represent
/// <a href="https://discord.com/assets/43bef54c8aee2bc0fd1c717d5f8ae28a.png">represent</a>
/// </summary>
/// <remarks>images must be registered with the developer portal to use them, to use them return the name of the image</remarks>
public static class DiscordIntegration
{
    public static DiscordRpcClient discord;
    public static bool discordAlive;
    public static DateTime now;
    public static Func<string> details;
    public static Func<string> state;
    public static Func<string> largeImage;
    public static Func<string> largeText;
    public static Func<string> smallImage;
    public static Func<string> smallText;

    public static void Init() => now = DateTime.UtcNow;

    public static void CheckDiscord(string appId, bool retry = true)
    {
        if (appId == string.Empty) return;
        discordAlive = true;
        try
        {
            discord = new DiscordRpcClient(appId, autoEvents: false)
            {
                SkipIdenticalPresence = true
            };
            discord.Initialize();

            RichPresence rp = new();
            var assets = rp.Assets = new Assets();
            if (details is not null) rp.Details = details.Invoke();
            if (state is not null) rp.State = state.Invoke();
            if (largeImage is not null) assets.LargeImageKey = largeImage.Invoke();
            if (largeText is not null) assets.LargeImageText = largeText.Invoke();
            if (smallImage is not null) assets.SmallImageKey = smallImage.Invoke();
            if (smallText is not null) assets.SmallImageText = smallText.Invoke();

            discord.SetPresence(rp);
            UpdateActivity();
            WriteToConsole($"{CYAN}Discord Connected");
        }
        catch (Exception e)
        {
            Logger.Log(Warning, $"DISCORD ERR: {e}");
            WriteToConsole($"{RED}Discord Failed to connect");
            discordAlive = false;

            if (retry)
            {
                WriteToConsole($"{YELLOW}Retrying Discord connection");
                Logger.Log(Debug, "RETRYING TO CHECK IF FLUKE");
                CheckDiscord(appId, false);
            }
            else WriteToConsole($"{DARKRED}Retry failed, use the 'discord' command to retry again");
        }
    }

    public static void UpdateActivity()
    {
        try
        {
            if (!discordAlive) return;
            discord.UpdateStartTime(now);
            if (details is not null) discord.UpdateDetails(details.Invoke());
            if (state is not null) discord.UpdateState(state.Invoke());
            if (largeImage is not null && largeText is not null)
                discord.UpdateLargeAsset(largeImage.Invoke(), largeText.Invoke());
            if (smallImage is not null && smallText is not null)
                discord.UpdateSmallAsset(smallImage.Invoke(), smallText.Invoke());
            discord.Invoke();
        }
        catch (Exception e)
        {
            Logger.Log(Warning, $"DISCORD ERR: {e}");
            WriteToConsole($"{RED}Discord connection threw error");
            discordAlive = false;
        }
    }

    public static void Dispose()
    {
        if (discordAlive) discord.Dispose();
    }
}