using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class VariantSelect : ComponentInteractionModule<StringMenuInteractionContext>
{
    [ComponentInteraction("variantSelect")]
    public async Task SelectVariant()
    {
        //List<ComponentProperties> components = Context.Message.Components.Select(c => c.).ToList();

        //for (int i = 0; i < components.ActionRows.Count; i++)
        //{
        //    ActionRowBuilder? row = components.ActionRows[i];
        //    var selectMenu = row.Components.OfType<SelectMenuComponent>().FirstOrDefault();

        //    if (selectMenu != null)
        //    {
        //        var selectMenuBuilder = selectMenu.ToBuilder();

        //        foreach (var item in selectMenuBuilder.Options)
        //        {
        //            var selected = userSelection.Contains(item.Value);

        //            item.WithDefault(selected);
        //        }

        //        components.ActionRows[i] = new ActionRowBuilder().WithSelectMenu(selectMenuBuilder);
        //    }
        //}
        
        var menu = Context.Message.Components.OfType<StringMenu>().First();
        foreach (var option in menu.Options)
        {
            if (option.Value == Context.SelectedValues.First())
            {
            }
            else
            {
                option.Default = false;
            }
        }

        List<ComponentProperties> converter = Context.Message.Components.OfType<StringMenu>().ToList();
        foreach (var component in converter.OfType<StringMenu>())
        {
            component.
        }

        var callback = InteractionCallback.ModifyMessage(m =>
        {
            m.Components = converter;
            foreach (var item in menu)
            {
                if (item.CustomId == Context.SelectedValues.First())
                {
                    Console.WriteLine("found");
                }
            }
        });
        await RespondAsync(callback);
    }
}
