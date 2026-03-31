# Path
A path for a card that's moving involves 1 or more steps, and each step has a starting and ending location that's different. If your step is from a location to that same location, you didn't actually move anywhere. If you occupied multiple locations while moving, all of those locations are included in your path, including for oversized units.

Steps resolve one at a time. Because some abilities may change as your unit moves through the realm, you may declare seemingly-illegal movement paths. That is, the legality of the movement path is only checked step by step, as each individual step is about to happen. If the step is not legal at the moment you try to take it, you do not take that step, and you continue resolving anything remaining. See the examples below for further details.

When you declare movement, the only restriction is that you may not repeat specific steps. This means that a unit may move from location A, to location B, then back to location A (assuming it has enough movement), but it cannot then repeat the step of "A to B." Note that steps "A to B" and "B to A" are different steps.

If your unit takes at least one step, it has moved; if it takes zero steps, it has not moved.

A path for projectiles is a bit different, and can include just the starting location. See "((projectiles))" for more details.

## Example - Declaring a Seemingly Illegal Movement Path
_See live codex for table_

[[Rimland Nomads]] are at A, in a [[Silence]] aura on the leftmost 2x2 area. You use their Move and Attack basic ability and declare the following movement path:

- Surface A -> Surface B
- Surface B -> Surface C

Since the Nomads are in a Silence aura, they do not currently have Movement +1 and therefore they wouldn't normally be able to take two steps. Nonetheless, you can declare the path and see what happens. Once they step onto B, they're out of the aura. The game tries to resolve the second step and notices that, yes, the Nomads do have the capacity to take a second step, so they do, and end up at C.

[Codex Entry](https://curiosa.io/codex?id=2ba56532-3ac0-41e8-a73f-1005cbefe8db)