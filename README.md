# BattleBases
A small game between two opposing bases. Each side is able to build units from an outpost to attack 
the opponent's units, outposts, and base. A unit is limited to the avenue it was built on and continuously
moves forward, only stopping if it encounters another unit, an outpost, or a base. If the unit encounters
an enemy, it begins fighting until it or its enemy is defeated. The final goal is to destroy the enemy
base.

This game is being completed in phases. Each phase is organized under its own scene in the Unity Editor.
Each phase is to be culmulative, adding on the previous phase.

Phase 1: Combat Manager
This phase focuses on getting the basics of the game adding and functioning. Once complete, two units will
match forward until they are in range to initiate combat. Combat will be managed by the Combat Manager.
Once a unit falls, it will resume it's normal behavior, i.e. advancing to the enemy side. Combat will
be based on a units attack power and attack speed.

Phase 2: Outposts
Outposts are tasked with spawning units for their side. If a unit is in range of an opposing outpost and
their isn't an enemy unit to defend it, the unit will begin attacking the outpost. An outpost can still
spawn new units, provided there is enough room. If an outpost is under attack and it spawns a new unit,
the enemy will initiate combat against the new unit and will not attack the outpost until combat is 
resolved.

Phase 3: Full Gameboard
At this point, it will be possible to construct a full, playable gameboard from the units, outposts, and 
avenues made in the previous phases. Each team will also be given a base at their side of the game board.
These bases will have a light attack that works on any enemy in range. This means that it can attack
enemies that are either not currently in combat with the base or even in combat with a unit or outpost.

Phase 4: Gamerules
With a complete gameboard, focus will shift to implementing game rules and win conditions. Units will need
to be bought using money from the team's treasury. Money is regularly added to the treasury over time and
is also rewarded for defeating an enemy unit or outpost. Once a base is destroyed the game is over, and the
team that still has a base is declared the winner.

Phase 5: Implement AI
Finally, an opposing AI will need to be built to play against the player. 
