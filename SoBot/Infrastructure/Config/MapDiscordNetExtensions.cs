// Copyright (c) Dominic Eliot.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace SorceryBot.Infrastructure.Config;
internal static class MapDiscordNetExtensions
{
    public static void MapDiscord(this IHost host)
    {
        var interactionService = host.Services.GetService<InteractionService>();
        var client = host.Services.GetRequiredService<DiscordSocketClient>();

        if (interactionService != null)
        {
            interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), host.Services);
        }
    }
}
