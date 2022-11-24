using DiscordRPC;
using RayWrapper.Base.GameBox;
using RayWrapper.Vars;
using static RayWrapper.Base.GameBox.AttributeManager;
using static RayWrapper.Base.GameBox.AttributeManager.PlacerType;
using static RayWrapper.Base.GameBox.Logger.Level;

namespace RayWrapper.Discord;

/// <summary>
/// the following is recommended to put in an the <see cref="Vars.GameLoop.Init()"/> method of the <see cref="Vars.GameLoop"/>
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
    public static string discordAppId = string.Empty;
    public static DiscordRpcClient discord;
    public static bool discordAlive;
    public static DateTime now;
    public static Func<string> details;
    public static Func<string> state;
    public static Func<string> largeImage;
    public static Func<string> largeText;
    public static Func<string> smallImage;
    public static Func<string> smallText;

    private static bool _initDiscord;

    public static void Init(string discordAppId)
    {
        if (_initDiscord) return;
        _initDiscord = true;
        DiscordIntegration.discordAppId = discordAppId;
        if (discordAppId != string.Empty) CheckDiscord(discordAppId);
        GameBox.AddScheduler(new Scheduler(100, UpdateActivity));
        now = DateTime.UtcNow;
    }

    public static void CheckDiscord(string appId, bool retry = true)
    {
        if (appId == string.Empty) return;
        discordAlive = true;
        try
        {
            discord.UpdateStartTime(now);
        }
        catch
        {
            // look man, idk 
        }

        try
        {
            discord = new DiscordRpcClient(appId, autoEvents: false)
            {
                SkipIdenticalPresence = true
            };
            discord.Initialize();

            RichPresence rp = new();
            var assets = rp.Assets = new Assets();
            if (details is not null) rp.Details = details();
            if (state is not null) rp.State = state();
            if (largeImage is not null) assets.LargeImageKey = largeImage();
            if (largeText is not null) assets.LargeImageText = largeText();
            if (smallImage is not null) assets.SmallImageKey = smallImage();
            if (smallText is not null) assets.SmallImageText = smallText();

            discord.SetPresence(rp);
            UpdateActivity();
            Logger.Log(Info, "Discord Connected");
        }
        catch (Exception e)
        {
            Logger.Log(Warning, $"DISCORD FAILED TO CONNECT: {e}");
            discordAlive = false;

            if (retry)
            {
                Logger.Log(Debug, "RETRYING TO CHECK IF FLUKE");
                CheckDiscord(appId, false);
            }
            else Logger.Log(Debug, "Retry failed, use the 'discord' command to retry again");
        }
    }

    public static void UpdateActivity()
    {
        
        try
        {
            if (!discordAlive) return;
            if (details is not null) discord.UpdateDetails(details());
            if (state is not null) discord.UpdateState(state());
            if (largeImage is not null && largeText is not null) discord.UpdateLargeAsset(largeImage(), largeText());
            if (smallImage is not null && smallText is not null) discord.UpdateSmallAsset(smallImage(), smallText());
            discord.Invoke();
        }
        catch (Exception e)
        {
            Logger.Log(Warning, $"DISCORD CONNECTION ERR: {e}");
            discordAlive = false;
        }
    }

    [GameBoxWedge(AfterDispose)]
    public static void Dispose()
    {
        if (discordAlive) discord.Dispose();
    }
}