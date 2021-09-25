using System;
using DiscordRPC;

namespace RayWrapper
{
    public class DiscordIntegration
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
        
        public static void CheckDiscord(string appId)
        {
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
            }
            catch (Exception e)
            {
                Console.WriteLine($"DISCORD ERR: {e}");
                discordAlive = false;
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
                Console.WriteLine($"DISCORD ERR: {e}");
                discordAlive = false;
            }
        }

        public static void Dispose()
        {
            if (discordAlive) discord.Dispose();
        }
    }
}