using Discord;
using Discord.Interactions;

namespace SorceryBot.DiscordUi;

public class VariantSelect : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("variantSelect")]
    public async Task SelectVariant(string[] userSelection)
    {
        if (!(Context.Interaction is IComponentInteraction { } interaction)) throw new ArgumentNullException(nameof(Context.Interaction));

        var components = ComponentBuilder.FromComponents(interaction.Message.Components);

        for (int i = 0; i < components.ActionRows.Count; i++)
        {
            ActionRowBuilder? row = components.ActionRows[i];
            var selectMenu = row.Components.OfType<SelectMenuComponent>().FirstOrDefault();

            if (selectMenu != null)
            {
                var selectMenuBuilder = selectMenu.ToBuilder();

                foreach (var item in selectMenuBuilder.Options)
                {
                    var selected = userSelection.Contains(item.Value);

                    item.WithDefault(selected);
                }

                components.ActionRows[i] = new ActionRowBuilder().WithSelectMenu(selectMenuBuilder);
            }
        }

        await interaction.UpdateAsync(msg =>
        {
            msg.Components = components.Build();
        });
    }
}
