using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class VariantSelect : ComponentInteractionModule<StringMenuInteractionContext>
{
    [ComponentInteraction("variantSelect")]
    public async Task SelectVariant()
    {
        //Todo: Maybe just reuse the code to create the message, and modify the selected value.
        var components = ToEditableComponents(Context.Message.Components);
        var menu = components.OfType<StringMenuProperties>().First();

        foreach (var option in menu.Options)
        {
            option.Default = option.Value.Equals(Context.SelectedValues.First(), StringComparison.OrdinalIgnoreCase);
        }

        var callback = InteractionCallback.ModifyMessage(m =>
        {
            m.Components = components;
        });
        await RespondAsync(callback);
    }

    private List<ComponentProperties> ToEditableComponents(IEnumerable<IComponent> components)
    {
        var componentList = new List<ComponentProperties>();
        foreach (var messageComponent in components)
        {
            if (messageComponent is ActionRow ar)
            {
                var newActionRow = new ActionRowProperties();
                newActionRow.AddButtons(ar.Buttons.OfType<Button>()
                    .Select(b => new ButtonProperties(b.CustomId, b.Label, b.Style)
                    {
                        Disabled = b.Disabled,
                        Emoji = (b.Emoji?.Id != null) ? new EmojiProperties(b.Emoji.Id.Value) : null
                    }
                ));
                componentList.Add(newActionRow);
            }

            else if (messageComponent is StringMenu sm)
            {
                var newMenu = new StringMenuProperties(sm.CustomId);
                newMenu.AddOptions(sm.Options
                    .Select(o => new StringMenuSelectOptionProperties(o.Label, o.Value)
                    {
                        Default = o.Default,
                        Description = o.Description,
                        Emoji = (o.Emoji?.Id != null) ? new EmojiProperties(o.Emoji.Id.Value) : null
                    }
                ));
                componentList.Add(newMenu);
            }

            else
            {
                throw new ArgumentException($"Unknown type {messageComponent.GetType().FullName}");
            }
        }

        return componentList;
    }
}
