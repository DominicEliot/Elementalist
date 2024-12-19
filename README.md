# SorceryBot
This is a discord bot for the TCG Sorcery. It was designed to use as many discord features I could think of to make finding the card, and getting the right feature as easy as possible. It makes heavy use of slash commands, and discord embeds/buttons.

## Contributing
Contributions are welcome, from fully coded features, to feature requests, and issues.

## Building
The bot is written in c# and uses .Net 9. The main solution is the SLNX file. Visual Studio 2022 requires that you enable preview feature to get this to work. From there building should be pretty straight forward.

## Running the bot
You will need a bot token provided from discord. Place this in a file call `BotToken.Private.json` in the bot's root directory.

## Code
The code in this project is intentionally over-engineered. I did it this way to learn some architecture designs than to be the most straight forward code.
It is using:
 * A version of the Vertical Slice Architecture, that I adapted to work a little better for a discord bot.
 * CQRS with MediatR
 * Discord.Net to facilitate communications with the discord API
