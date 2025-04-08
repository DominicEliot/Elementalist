using System.Text.Json;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Models;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class VariantSelect(ICardRepository cardRepository) : ComponentInteractionModule<StringMenuInteractionContext>
{
    private readonly ICardRepository _cardRepository = cardRepository;

    [ComponentInteraction("variantSelect")]
    public async Task SelectVariant()
    {
        var uniqueCard = JsonSerializer.Deserialize<UniqueCardIdentifier>(Context.SelectedValues[0]);

        if (uniqueCard == null)
        {
            await RespondAsync(InteractionCallback.Message(new() { Content = "Unknown card data format", Flags = MessageFlags.Ephemeral }));
            return;
        }

        var card = (await _cardRepository.GetCardsMatching(c => c.Name == uniqueCard.Name)).First();
        var set = card.Sets.First(s => s.Name == uniqueCard.Set);
        var variant = set.Variants.First(v => v.Product == uniqueCard.Product && v.Finish == uniqueCard.Finish);
        var setVariant = new SetVariant() { Set = set, Variant = variant };

        var message = CardDisplay.CardInfoMessage([card], setVariant);

        var callback = InteractionCallback.ModifyMessage(m =>
        {
            m.Components = message.Components;
            m.Embeds = message.Embeds;
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
