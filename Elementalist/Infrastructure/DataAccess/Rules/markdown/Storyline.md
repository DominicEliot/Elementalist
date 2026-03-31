# Storyline
When the story being told during a game of _Sorcery_ starts to get complicated, particularly when multiple abilities trigger at the same time, use the storyline to resolve them.

## Most Effects Use The Storyline
A storyline begins each time a spell is cast, or an ability is activated or triggered.

For example, when you cast the [[Plumed Pegasus]] spell, the spell goes onto the storyline, and then it resolves from the storyline. The same is true for abilities. For example, when you tap a unit to use the "Move and Attack" basic ability, the ability itself goes onto the storyline, then resolves from the storyline.

Minion, aura, and artifact spells enter the realm after they resolve, while magic spells go to the cemetery after they resolve.

When casting spells, or activating or triggering abilities, please note the "target" rules.

## Some Effects Do Not Use the Storyline
Note that replacement effects, prevention effects, and ongoing effects do not use the storyline. See those codex entries for further details.

## Splitting Events on the Storyline
As some events are resolving from the storyline, they split up into smaller events. Note that events are only split up as they are resolving, and not when they are added to the storyline. The following are the reasons to split up an event as it resolves from the storyline:

- A card enters or leaves the realm
- A card moves in the realm
- Explicit card text, e.g. "one at a time" on [[Firebolts]] or [[Tactical Move]]

For example, let's say the realm looks like this:

_See live codex for table_

I have an allied at [[Azuridge Caravan]] at A. Site R is a [[Summer River]]. There's an enemy [[Lady Iseult]] at I.

I cast [[Grapple Shot]]. The storyline will look like this:

- Grapple Shot effect

If there were any cast triggers (e.g. [[Enchantress]], [[Fenvale Muse]], [[Ring of Morrigan]], etc.), those ((new events interrupt the storyline)). Assuming for now we don't have those, Grapple Shot starts to resolve.

I choose Azuridge Caravan as the ally, who shoots a projectile, hitting Lady Iseult. At this moment, the Caravan needs to move, so we handle the ((movement on the storyline)). The movement path is set, and now Grapple Shot is split into smaller events, so the storyline now looks like this:

- DONE: _first part of Grapple Shot_
- Azuridge Caravan moves from surface of A to surface of R
- Azuridge Caravan moves from surface of R to surface of I
- Remaining part of Grapple Shot

The events on the storyline continue to resolve now, and triggered abilities may insert themselves as needed, since ((new events interrupt the storyline)).

**Another Example of Splitting Events  
**Let's say I use the [[Druid]]'s ability to play a site and summon Tawny. I tap the Druid and the storyline looks like this:

- Druid's ability (Play or draw a site. If you played a site and you don't control Tawny, summon her there.)

So, the ability starts to resolve and I play my site. As soon as a card enters the realm, the event on the storyline splits, so it now looks like:

- DONE: _first part of Druid's ability (Play or draw a site)_
- Remaining part of Druid's ability (If you played a site and you don't control Tawny, summon her there.)

Since ((new events interrupt the storyline)), here's what the storyline would look like if the site had a genesis ability, e.g. [[Red Desert]]:

- DONE: _first part of Druid's ability (Play or draw a site)_
- Red Desert's genesis ability
- Remaining part of Druid's ability (If you played a site and you don't control Tawny, summon her there.)

The Red Desert's genesis ability is inserted before the remaining part of the Druid's ability. The Red Desert's genesis ability must resolve before Tawny is summoned.  

**Special Case for Search Effects  
**If an effect causes you to search, then do something that would normally cause a storyline event to be split, and then do something else with the searched cards, you complete the entire search process before splitting the event.

For example, if [[Brother Knight]]'s genesis ability is resolving and I search my deck, then summon the other Brother Knight, normally a card entering the realm would cause the event to split on the storyline. E.g. a [[Giant Shark]] or [[Bottomless Pit]] might interrupt the rest of the first Brother Knight's genesis ability. But, in fact, you must complete the entire search process and shuffle your deck before allowing any triggered abilities to insert themselves into the storyline. The Giant Shark or Bottomless Pit could absolutely still trigger, but your deck would be shuffled before they do!

## Movement on the Storyline
When you need to specify movement from an effect on the storyline, the entire movement path goes onto the storyline before any movement actually resolves. Each step is resolved one at a time, but the whole movement path is placed on the storyline first.

See ((Splitting Events on the Storyline)) for an example of [[Grapple Shot]], and see ((Projectiles That Cause Forced Movement)) for more details.

Some effects cause movement to a specific location without specifying the path, e.g. [[Giant Shark]]. When such an effect resolves from the storyline, you must specify the shortest path that would allow the unit to actually reach the desired destination, ignoring triggered abilities. If there are multiple such paths, the controller of the effect chooses among the tied routes. If there is no valid path that allows the unit to reach its destination (e.g. [[Iceberg]]), do not move at all.

## New Events Interrupt The Storyline
If the resolution of one event triggers a new event, the new event will be inserted into the storyline just before the triggering event. The new event temporarily interrupts any remaining events on the storyline. Once the new event has been resolved, the story continues.

For example, if you cast [[Plumed Pegasus]] while carrying the [[Ring of Morrigan]], the sequence will be:

**1)** Cast Plumed Pegasus. The storyline looks like this:

- Summon Plumed Pegasus to the surface of site 8

**2)** Ring of Morrigan notices the "cast" trigger, and now the storyline looks like this:

- Ring of Morrigan ability
- Summon Plumed Pegasus to the surface of site 8

**3) **Then, you resolve the Ring of Morrigan ability. Note that the Plumed Pegasus spell hasn't resolved yet, so it's still on the storyline and not in the realm. The storyline looks like this:

- DONE: _Ring of Morrigan ability_
- Summon Plumed Pegasus to the surface of site 8

**4) **Then, the Plumed Pegasus spell resolves. The storyline looks like this:

- DONE: _Ring of Morrigan ability_
- DONE: _Summon Plumed Pegasus to the surface of site 8_

And the Plumed Pegasus is now in the realm.

## Simultaneous Triggers
If multiple events are competing to be added at the same point on the storyline, the active player adds all of their events first, in the order of their choice. Then, the non-active player add their events in the order of their choice. The non-active player's events will end up resolving first, since newly added events always interrupt what's already on the storyline. See the example below.

It's my turn and I cast [[Apprentice Wizard]] to the surface of my [[Bottomless Pit]]. Also, my Bottomless Pit happens to be flooded and you have a [[Mariner's Curse]] there. The storyline starts like this:

- Summon Apprentice Wizard to the surface of Bottomless Pit

There are no triggers yet, because the Wizard isn't the realm yet. So, we resolve the storyline and it looks like this:

- DONE: _Summon Apprentice Wizard to the surface of Bottomless Pit_

Now, the Apprentice Wizard is in the realm, and we have three simultaneous triggers! As the active player, I declare mine first in the order of my choice. I order them like this:

- Apprentice Wizard Genesis (Draw a spell)
- Bottomless Pit ability (Kill Apprentice Wizard since it doesn't have Airborne)

Then, as the non-active player, you declare your ability and add it ahead of mine. The storyline now looks like this:

- Mariner's Curse ability (Submerge Apprentice Wizard and return to hand)
- Apprentice Wizard Genesis (Draw a spell)
- Bottomless Pit ability (Kill Apprentice Wizard since it doesn't have Airborne)

And then those three effects will resolve in order, yours first, then mine. See the "((Source is No Longer in the Realm))" example below for more details.

## Simultaneous Effects
Sometimes multiple effects occur simultaneously, as a single event on the storyline. For example, [[Windblast]] involves many cards moving, but they all move simultaneously and are treated as a single event on the Storyline.

Simultaneous effects are different from ((simultaneous triggers)). Effects are simultaneous and are resolved as a single event on the storyline if they use language like "everything." E.g. [[Peasant Revolt]] can simultaneously destroy many cards. An effect is also simultaneous if it uses language like "each" or "all," without also using "one at a time." For example, [[Maelström]] simultaneously moves many minions and [[Boneyard]] simultaneously summons two minions, but [[Tactical Move]] is resolved as separate events on the storyline.

## Source is No Longer in the Realm
If the source of a storyline event was in the realm but then leaves the realm before the event resolves, the event is ignored and does not actually resolve. Simply proceed to the next event on the storyline.

Continuing from the example in "((Multiple Simultaneous Triggers))" above, the [[Apprentice Wizard]] entered the realm on the surface of a flooded [[Bottomless Pit]] with a [[Mariner's Curse]]. We have three events on the storyline, as follows:

- Mariner's Curse ability (Submerge Apprentice Wizard and return to hand)
- Apprentice Wizard Genesis (Draw a spell)
- Bottomless Pit ability (Kill Apprentice Wizard since it doesn't have Airborne)

The Mariner's Curse resolves, submerging the Apprentice Wizard (who dies underwater without the submerge ability), and the Mariner's Curse returns to your hand. Now, the storyline looks like:

- DONE: _Mariner's Curse ability_
- Apprentice Wizard Genesis (Draw a spell)
- Bottomless Pit ability (Kill Apprentice Wizard since it doesn't have Airborne)

Now, the Apprentice Wizard's Genesis ability tries to resolve, but the Apprentice Wizard is no longer in the realm. The genesis ability cannot resolve and is ignored. I do not draw a spell. Now, the storyline looks like:

- DONE: _Mariner's Curse ability_
- IGNORED: _Apprentice Wizard Genesis_
- Bottomless Pit ability (Kill Apprentice Wizard since it doesn't have Airborne)

The Bottomless Pit ability resolves. It tries to kill the Apprentice Wizard and fails, since the Apprentice Wizard is already dead.

If a specific event has started to resolve and got split up (per the ((Splitting Events on the Storyline)) rules), that event will continue to resolve even if the source leaves the realm.

[Codex Entry](https://curiosa.io/codex?id=081fc13c-49fb-4893-90c1-1ba5324e6241)