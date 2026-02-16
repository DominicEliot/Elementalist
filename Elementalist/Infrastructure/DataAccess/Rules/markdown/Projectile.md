# Projectile
A projectile’s flight begins at the shooting unit’s location and travels within the same region in a cardinal direction. Its flight continues until it either reaches the edge of its region or it reaches the first unit (enemy or ally) along its path, ignoring any allies at the projectile’s starting location.

Different projectiles will have different effects, but often deal damage to the impacted unit.

If there are multiple valid units that could be hit, the projectile's controller chooses which unit the projectile hits.

Projectiles enter their starting location. Then, they have a path that includes their starting location and any additional locations.

Projectiles cannot hit minions with Stealth, but some projectiles don't care about "hitting" and instead just care about their path (e.g. [[Snowball]]).

## Piercing Projectile
Piercing projectiles keep moving even after hitting a unit; they only stop when they reach a region boundary.

Piercing projectiles only hit one unit at each location, ignoring allies at the starting location as usual.

A piercing projectile cannot cross region boundaries, and cannot hit a unit more than once (even if it’s oversized).

## Projectiles That Cause Forced Movement
Some projectiles cause forced movement after they hit, e.g. [[Grapple Shot]] or [[Pudge Butcher]]. The forced movement path matches the projectile's path. (If [[Magellan Globe]] or [[Ruler of Thul]] is in the realm, you can't shoot the projectile one way, and then drag the other way around the realm.)

See the "((Movement on the Storyline))" entry for more details.

[Codex Entry](https://curiosa.io/codex?id=c81c8875-ef70-4202-8c98-7ac00ece0cc6)