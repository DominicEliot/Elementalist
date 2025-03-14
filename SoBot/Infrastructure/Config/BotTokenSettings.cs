// Copyright (c) Dominic Eliot.  All rights reserved.

using Discord;

namespace Elementalist.Infrastructure.Config;

public class BotTokenSettings
{
    public required string Token { get; set; }
    public TokenType TokenType { get; set; } = TokenType.Bot;
    public required string ClientId { get; set; }
}
