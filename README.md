# Elementalist
The Elementalist is a discord bot for the TCG Sorcery. It was designed to use as many discord features I could think of to make finding the card, and getting the right information as easy as possible. It makes heavy use of slash commands, and discord embeds/buttons.

## Getting started
There are two ways to start using the bot, you can use both if you want.
1. Adding the bot to your personal applications. This is great because you can invoke bot commands on any discord server that you are a member of.
1. Add the bot as a server wide bot. This is the traditional approach, you must be an admin on the server you wish to add the bot to. This is a good option because it lets everyone ona server use the bot without having to know about it first.

Use this link to add the bot either as a personal application or as a server wide bot:
https://discord.com/oauth2/authorize?client_id=1306685996984827924
_Pro tip: you can also add the bot from the bot's discord profile icon_

From there you can start using the bot. To discover the commands it accepts just type `/` in the chat, or open a direct message with the bot.
A good starter command is `/card-by-name Elementalist`

Warning: Due to discord's newish limitation on bot's ability to see chat messages this bot cannot see/respond to messages with something like `[[Card Name]]` directly in the message like magic's scryfall bot can. This might be added in the future if discord approves my application for viewing message content.

## Contributing
Contributions are welcome, from fully coded features, to feature requests, and issues.

# Editing the source code
Most users don't need to worry about everything below this. This is only for people looking to host the bot on thier own hardware, or contibute to the project with bug fixes, and new features.

## Running the bot with Docker
You will need a bot token provided from discord.
Get the docker container from docker hub, or run the Dockerfile after cloning this repository.
```
docker pull 2masgllrpxbruv/elementalist-bot:latest
```
Set the `DISCORD__TOKEN` environment variable with `docker run -e DISCORD__TOKEN=your-token-from-discord image-name`

## Building
The bot is written in c# and uses .Net 9. The main solution is the SLNX file. If you have issues opening the solution make sure your IDE is fully up to date.
Additionally you need to setup the bot token with the following command: `dotnet user-secrets set "DISCORD:TOKEN" "your-token-from-discord"`
From there building should be pretty straight forward.

## Code
The code in this project is intentionally over-engineered. I did it this way to learn some architecture designs more than to be the most straight forward code.
It is using:
 * A version of the Vertical Slice Architecture, that I adapted to work a little better for a discord bot.
 * CQRS with MediatR
 * FluentValidation
 * [NetCord](https://netcord.dev/) to facilitate communications with the discord API
