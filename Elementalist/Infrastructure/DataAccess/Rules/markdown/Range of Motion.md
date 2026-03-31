# Range of Motion
A unit's range of motion is every location it could reach if it used the Move and Attack basic ability, ignoring all triggered abilities. If a unit can't use the Move and Attack basic ability (e.g. Disabled), then it has no range of motion.

When determining which locations the unit can reach (and thus Defend there, or be forced to attack there via [[The Green Knight]], for example), the game looks ahead, considering all ongoing effects (including passive abilities) and _ignoring_ all triggered abilities. Below are some examples to help illuminate these rules.

**Example 1**

_See live codex for table_

An [[Apprentice Wizard]] is in square A and the attack is happening in square E. The game foresees that the Apprentice Wizard can't reach square E and thus he can't tap to Defend.

**Example 2**  
Same as Example 1, but [[Magellan Globe]] is in the realm or [[Waypoint Portal]] is in effect. Now, the game notices the ongoing effect and allows the Defend ability.

**Example 3**  
The attack's location is [[Gnome Hollows]] and a [[Bosk Troll]] is at an adjacent site. The game foresee that the Bosk Troll won't be able to move into the Gnome Hollows because that's a passive effect, and thus the Troll can't tap to Defend.

**Example 4**  
The attack's location is [[Bottomless Pit]] and the Bosk Troll is adjacent. The game _will_ _allow _the Defend ability, because the Troll can actually move there. Of course, the Bosk Troll will die upon arrival due to Bottomless Pit, but the Defend ability would be allowed.

**Example 5**

_See live codex for table_

The attack is happening at A. I have [[Rimland Nomads]] at N. There is a [[Giant Shark]] at S, which is a water site. As soon as the Rimland Nomads move toward A to Defend, the Shark will fight them at S via its mandatory triggered ability. There is no way that the Nomads could actually reach A in two steps. Nonetheless, the game _will_ _allow _the Rimland Nomads to use the Defend ability because when the game looks ahead to see which locations the Nomads can reach, the game ignores all triggered abilities.

**Example 6  
**Same as Example 5, but instead of a Shark at S, it's a [[Bog]], having targeted itself. In this case, the game will _not_ _allow _the Nomads to use the Defend ability, because the game considers all ongoing effects (including the Bog) and foresees that the Nomads can't reach the attack's location. The difference is that Giant Shark is a triggered ability (which is ignored during lookahead) and Bog is an ongoing ability (which is considered during lookahead).

**Lookahead Summary  
**In summary, if the player can demonstrate a movement path that would reach the attack's location, ignoring all triggered abilities, then the Defend ability is allowed.

**Explanation  
**If you're reading this deeply into the Codex, you might be wondering why the game ignores triggered abilities for the lookahead. One reason is that there are some cases where the lookahead for triggered abilities yields an unknown outcome. For example, in Example 5, instead of Nomads, if it were a [[Phantom Steed]] with a [[Poisonous Dagger]] carrying [[Amazon Warriors]], can the Phantom Steed reach A or not? It depends if the Shark chooses to fight the Warriors first or the Steed first. The second reason is that this approach allows the lookahead for mandatory actions like "must attack" (e.g. [[Mask of Mayhem]]) to use the same rules, since players' intuition in those cases is to force an attack even into a Shark or [[Mariner's Curse]]. Therefore, we consistently ignore triggered abilities when considering a unit's range of motion.

[Codex Entry](https://curiosa.io/codex?id=baf60fbb-dcd9-4bb5-a2b4-158f8df5cbb1)