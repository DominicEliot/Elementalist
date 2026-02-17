using NetCord;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public static class DiscordContextExtensions
{
    public static ulong? GetGuildId(this IGuildContext context)
    {
        if (context.Guild?.Id is not null)
        {
            return context.Guild.Id;
        }

        if (context is not IChannelContext channelContext)
        {
            return null;
        }

        var channel = channelContext.Channel as IGuildChannel;
        return channel?.GuildId;
    }
}
