# SorceryBot
This is a discord bot for the TCG Sorcery. It was designed to use as many discord features I could think of to make finding the card, and getting the right feature as easy as possible. It makes heavy use of slash commands, and discord embeds/buttons.

## Contributing
Contributions are welcome, from fully coded features, to feature requests, and issues.

## Running the bot with Docker
You will need a bot token provided from discord.
Get the docker container from docker hub, or run the Dockerfile after cloning this repository.
```
docker pull 2masgllrpxbruv/elementalist-bot:latest
```
Set the `BOT_TOKEN` environment variable with `docker run -e BOT_TOKEN='your-token-from-discord'`

## Building
The bot is written in c# and uses .Net 9. The main solution is the SLNX file. Visual Studio 2022 requires that you enable preview feature to get this to work.
Additionally you need to setup the bot token with the following command: `dotnet user-secrets set "BOT_TOKEN" "your-token-from-discord"`
From there building should be pretty straight forward.

## Code
The code in this project is intentionally over-engineered. I did it this way to learn some architecture designs than to be the most straight forward code.
It is using:
 * A version of the Vertical Slice Architecture, that I adapted to work a little better for a discord bot.
 * CQRS with MediatR
 * FluentValidation
 * Discord.Net to facilitate communications with the discord API
