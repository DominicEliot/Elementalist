using System.Text.Json;
using Elementalist.Infrastructure.DataAccess.CardData;

namespace SorceryBotTests.Features.Card;

internal class MockCardRepository : ICardRepository
{
    private List<Elementalist.Models.Card> _cards = [
        new("Pudge Butcher", null!, "Earth", "Minion Demon", []),
        JsonSerializer.Deserialize<Elementalist.Models.Card>("""{ "name": "Hounds of Ondaros", "guardian": { "rarity": "Elite", "type": "Minion", "rulesText": "Airborne, Burrowing, Submerge, Voidwalk\r\n \r\nNearby enemies permanently lose Stealth.", "cost": 5, "attack": 4, "defence": 4, "life": null, "thresholds": { "air": 2, "earth": 0, "fire": 0, "water": 0 } }, "elements": "Air", "subTypes": "Beast", "sets": [ { "name": "Alpha", "releasedAt": "2023-04-19T00:00:00.000Z", "metadata": { "rarity": "Elite", "type": "Minion", "rulesText": "Airborne, Burrowing, Submerge, Voidwalk\n\nNearby enemies permanently lose Stealth.", "cost": 5, "attack": 4, "defence": 4, "life": null, "thresholds": { "air": 2, "earth": 0, "fire": 0, "water": 0 } }, "variants": [ { "slug": "alp_hounds_of_ondaros_b_s", "finish": "Standard", "product": "Booster", "artist": "Francesca Baerald", "flavorText": "From hell's heart they stab at thee.", "typeText": "Nowhere is safe from these Elite Beasts" }, { "slug": "alp_hounds_of_ondaros_b_f", "finish": "Foil", "product": "Booster", "artist": "Francesca Baerald", "flavorText": "From hell's heart they stab at thee.", "typeText": "Nowhere is safe from these Elite Beasts" } ] }, { "name": "Beta", "releasedAt": "2023-11-10T00:00:00.000Z", "metadata": { "rarity": "Elite", "type": "Minion", "rulesText": "Airborne, Burrowing, Submerge, Voidwalk\r\n \r\nNearby enemies permanently lose Stealth.", "cost": 5, "attack": 4, "defence": 4, "life": null, "thresholds": { "air": 2, "earth": 0, "fire": 0, "water": 0 } }, "variants": [ { "slug": "bet_hounds_of_ondaros_b_s", "finish": "Standard", "product": "Booster", "artist": "Francesca Baerald", "flavorText": "From hell's heart they stab at thee.", "typeText": "Nowhere is safe from these Elite Beasts" }, { "slug": "bet_hounds_of_ondaros_b_f", "finish": "Foil", "product": "Booster", "artist": "Francesca Baerald", "flavorText": "From hell's heart they stab at thee.", "typeText": "Nowhere is safe from these Elite Beasts" } ] } ] }""", JsonSerializerOptions.Web),
        JsonSerializer.Deserialize<Elementalist.Models.Card>("""{ "name": "Wolpertinger", "guardian": { "rarity": "Elite", "type": "Minion", "rulesText": "Has +1 power for each of your dead Beasts.", "cost": 4, "attack": 0, "defence": 0, "life": null, "thresholds": { "air": 0, "earth": 2, "fire": 0, "water": 0 } }, "elements": "Earth", "subTypes": "Beast", "sets": [ { "name": "Arthurian Legends", "releasedAt": "2024-10-04T00:00:00.000Z", "metadata": { "rarity": "Elite", "type": "Minion", "rulesText": "Has +1 power for each of your dead Beasts.", "cost": 4, "attack": 0, "defence": 0, "life": null, "thresholds": { "air": 0, "earth": 2, "fire": 0, "water": 0 } }, "variants": [ { "slug": "art_wolpertinger_b_s", "finish": "Standard", "product": "Booster", "artist": "Séverine Pineaux", "flavorText": "Fur friends and family, we remember you.", "typeText": "An Elite Beast of tooth, claw, and horn" }, { "slug": "art_wolpertinger_b_f", "finish": "Foil", "product": "Booster", "artist": "Séverine Pineaux", "flavorText": "Fur friends and family, we remember you.", "typeText": "An Elite Beast of tooth, claw, and horn" } ] } ] }""", JsonSerializerOptions.Web),
        ];

    public Task<IEnumerable<Elementalist.Models.Card>> GetCards()
    {
        return Task.FromResult(_cards.AsEnumerable());
    }

    public Task<IEnumerable<Elementalist.Models.Card>> GetCardsMatching(Func<Elementalist.Models.Card, bool> predicate)
    {
        return Task.FromResult(_cards.Where(predicate));
    }
}
