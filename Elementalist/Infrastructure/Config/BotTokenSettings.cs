// Copyright (c) Dominic Eliot.  All rights reserved.

using Discord;

namespace Elementalist.Infrastructure.Config;

public class BotTokenSettings
{
    [ConfigurationKeyName("BOT_TOKEN")]
    public required string Token { get; set; }
    [ConfigurationKeyName("BOT_TOKEN_TYPE")]
    public TokenType TokenType { get; set; } = TokenType.Bot;
    [ConfigurationKeyName("BOT_CLIENT_ID")]
    public required string ClientId { get; set; }
}
