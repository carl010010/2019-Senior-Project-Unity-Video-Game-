# 2019 Senior Project The study of the shadowed: Theif (Unity Video Game)

Here lies a years worth of work. Skip the reading and just play the [game](#play-my-game).

My school [Oregon Institute of Technology](https://www.oit.edu/) reqired each student on the computer science BS track to complete an individual 3 term long project. Thusly named a Senior project.

For my Senior project I choose to create a video game, loosely based off of the [Thief series](https://en.wikipedia.org/wiki/Thief_(series)).



While was my first ever "complete" game, I have had many years of experimenting in unity, and I know a full game would be quite a difficult challange. For that reason and that fact that i'm a software enginer I set some limits on the scope of the "art" in  the project.


	1.  I wasnt going to worry about graphics or animations (Cubes and flat textures were fair game)
	2.  The UI only needs to be functional
	3.  Not to worry about sounds other then foot steps 
	4.  If the games not pretty it better work

# 6 Main Pillars for success

Because my senoir project required quite a bit of decumentation and forthouhght, I settled on 6 things that I needed to complete to be able to call my project complete.

Each having a subset of things that would be used as a set of goals for each pillar 

### 1. Shadow System
A system to detect how far away the player is from a light based on the light’s range and intensity.
Used to see how much the player is in the light (0% completely in shadow, 100% completely visible).
Used to determine if enemies are able to see the player.

### 2. Sound System
A system to detect how much sound a player is making (0% no sound not moving, 100% running on marble or metal floors) based off of how fast the player is moving and what type of surface they are walking on (e.g. marble floors are louder then grass or carpet)

Used to determine if enemies can hear the player

### 3. Enemy Detection

Enemies have “sight” to try and detect the player. First, we start by seeing if the enemy has a clear line of sight to the player and then if we can see the player we use shadow detection to see if the player is “visible” enough to the enemy. Visible being determined by how much the player is in light plus how far away the player is to determine if the player is detected.

Enemies have “hearing” to try and detect the player’s movement and location. Enemies try to “hear” the player by seeing how much sound the player is making plus how far away the player is to determine if the player is detected.

Enemies have proximity detection. Enemies can detect if a player is to close based on how close the player is (in arm's reach) and how fast the player is moving to determine if the player is detected.

Latly, enemies have the ability to alert and be alerted by other guards. When the player is detected by an enemy, that enemy will try to alert other nearby guards that they have found, seen, or heard the player. When a guard is alerted by other guards, they will move away from where they are and try to find the player.

### 4. Enemy Movement, Navigation, and Interaction

Creating and moving enemies around a level to patrol the level, find the player when they are alerted, or when the player is detected, to chase the player.

Allowing enemies to unlock, open and close doors.

Turn on and off lights.

### 5. Player Movment

Creating fluid and responsive actions to move the player:

	looking around
	walking and running in 8 directions (North, East, South, West, North East, etc.)
	crouching down to half the player’s height and to walk slower
	jumping
	limbing ladders, ropes, and chest high objects (e.g. walls, fences, windows).

### 6. Player Interaction
  Player Inventory

Creating player interactions with each level:

	opening doors
	picking locks and safes
	stealing gold and valuable objects (e.g. jewels, rings, necklaces, vases, small statues) in the level and off of enemies
	being able to walk up behind non-alerted enemies and when within arms reach press a button to knock out enemies
	being able to put out light sources
	being able to use switches (light switches or switches that can open and close doors, bridges, safes)
 

# Play my game!

[The study of the shadowed: Theif](http://carllowther-sp.s3.amazonaws.com.s3-website-us-west-2.amazonaws.com/)
