/* What needs to be done in the game */
/* The list is in no particular order */

// Note :
// When I say "DONE", I mean that it's working,
// but it may be very janky.

/* 
 * Do the Fuel and Hull (with consumption and damage) (DONE)
 * Then do the Refuel and Hull repair buildings (DONE)
 * Then do the Upgrades building (will require UI) (DONE)
 * Then start to work on the item upgrades (drill, fuel tank, hull, cargo size, propeller) (DONE)
 */

// Core Mechanics
// - Implement Digging (DONE)
// - * Add tile to inventory (DONE)
// - * Move Player to the center of the tile (DONE)
// - * * Add jitter animation to the pod, and maybe the camera as well (DONE)
// - * * Disable player movement input during the animation (DONE)
// - Inventory
// - * Allow possibility of discarding ores (to lower weight) (DONE)
// - * Show ore value/weight (DONE) -> might want to get total value instead of individual value)
// - Items (DONE)
// - Money (DONE)
// - Upgrades
// - * Drill Speed (DONE)
// - * Movement Speed (DONE, but it REALLY does not feel good, needs heavy refactoring)
// - * Fuel tank (DONE)
// - * Cargo size (DONE)
// - * Hull life (DONE)
// - Fuel consumption (DONE)
// - Hull life
// - * Fall damage (DONE)
// - * Dying/respawning (for fuel as well)
// - * * Steep money malus when dying (remove half or something)
// - * * Remove some amount of ore in the cargo (some or all of it)
// - * * * If all is lost, could have the wreck be recoverable where the player died (but the wreck would contain like half or whatever)
// - * * * Not sure how to handle when the player dies in the air. Fall down to the bottom ? Would the wreckage float ? (could go full silly with little balloons holding the wreckage in place)
// - * * * Could also be a simple game over, starting from last save or something, maybe with a hardcore mode with a single life
// - * * Fill fuel
// - * * The IMPORTANT part is to never be in a situation where the player is stuck without being able to make money/progress anymore
// - Stores
// - * Upgrades (DONE)
// - * Items (dynamite, teleport, etc) (DONE)
// - * Ore selling (DONE)
// - * Fuel buying (DONE)
// - * Hull repair (DONE)
// - Level saving
// - Level generation (DONE)
// - * Perlin Noise Implementation (DONE)
// - Pod weight based on ores in cargo bay
// - * Movement become sluggish, flying is slower or not possible anymore

// UI
// - Inventory (DONE)
// - Items (could be a part of the inventory screen) (gameplay implementation DONE, ui is trash)
// - Stores
// - Fuel/life (DONE)
// - Money (DONE)
// - Menus
// - * Main Menu
// - * Pause (DONE, not great)
// - * Options
// - Depth gauge
// - NPCs ? Dialogs ?
// - * Animate text apparition
// - * * Skip animation on click
// - * Go back to previous dialog box
// - * Witty stuff
// - * * If we go back too much, some NPCs could get annoyed that they need to repeat themselves ("go back" ? what do you mean, like a left arrow in the dialog box to re-read a previous text box ?)
// - * * If we skip too much, some NPCs could get annoyed that we don't listen to them
// - * * * Annoyance could be shown with a smaller more subtle dialog box, where the NPC grumbles a bit

// Misc
// - Add level bound, left and right (DONE - but requires visual collision)
// - Add floor on first level (DONE)
// - Tile properties
// - * digTime, weight, value, digLevel (which drill can dig it)
// - Secrets
// - * Flying above ground to a certain height
// - * Witty dialog in store after spamming the buy button on a sold-out item
// - * Potentially a secret black market after having bought every "standard" upgrades
// - Story line
// - * NPCs ?
// - * Side missions ?
// - Visuals
// - * Tile texture
// - * Pod
// - * Environment
// - * Stores
// - Monsters ?

// Balancing
// - The Cargo seems constantly too small
// - We can buy early upgrades way too easily
// Having less ores makes me more inclined to go towards ore pockets rather than ignoring and going deeper
// Same goes with speed, I prefer to go with what I see rather than going deeper
// I believe the implementation of slower digging by depth will increase that feeling even more.
// I think it's good, to an extent.
// Noise leaves a bit too many gaps, it should be a bit more uniform

// Figure out how to build the game for different platforms
// - PC
// - Android

// Add the Mouse Control (click to dig in the direction formed by the pod to the mouse, same for movement)
// For mobile, have two modes, either simulate mouse, or with the circle controllers


// Unity Project stuff
// https://www.youtube.com/watch?v=k3wHqPZUldw
// Assembly Definition
// Splits the code we write into different .dll to speed up compiles on reload
// Good info about .dll as well

// https://www.youtube.com/watch?v=p7GfSsQvR78
// Store data in text files and load it up

// https://learn.unity.com/tutorial/optimizing-unity-ui#
// UI optimisation





// Random notes

/*
 Questions about how to make the upgrades/consummable items

 items class
-name
-description
-cost
-icon

upgrade class (from items)
-value (capacity/speed)

consummable class (from items)
dynamite class (from consummable)
- blast radius
- does an action
teleport class (from consummable)
- does an action


Do I really want to make classes like that ?
If so, why don't I make the tiles like that ?
Each types of tiles are scriptableObjects
Can't I use that for the items ?
What's the point of one or the other ?
Maybe the fact that the consummable items does ACTIONS will require them to be classes
But for upgrades, it won't be necessary ?

*/




/*
need to check online how people store different items TYPE for their games
swords, shields, potions, food
and how their behavior are handled

https://www.youtube.com/watch?v=ZEdmFNfxFfk&list=PLcRSafycjWFegXSGBBf4fqIKWkHDw_G8D&index=16
https://www.youtube.com/watch?v=ZSdzzNiDvZk&list=PLJWSdH2kAe_Ij7d7ZFR2NIW8QCJE74CyT&index=5


https://www.youtube.com/watch?v=awUe44Rr4TU&list=PLcRSafycjWFegXSGBBf4fqIKWkHDw_G8D&index=3
how to make the UI, tricks about padding and whatnot


https://www.youtube.com/watch?v=M94bXfIcG6s&list=PLcRSafycjWFegXSGBBf4fqIKWkHDw_G8D&index=15
namespaces, good to avoid unwanted dependencies, encapsulate code
*/


// coolant upgrade ?
// would allow to drill through lava (ah, that's one more thing to add)
// maybe coolant would act as a depth barrier, must have a better coolant
// otherwise the heat will cause constant damage at certain depths

/*
 * Do I want a visible map, or would I want a radar, along with upgrades ?
 * Could have a sonar to reveal (some types of? or all?) ores
 * 
 * 
 * 
 */


/*
 * I'm thinking about different world, with different atmosphere, ores, upgrades
 * Perhaps our pod isn't good enough for the next world, so we need to upgrade more
 * Or we need a special pod for each world
 * But if we got multiple worlds with "cross-progression", one could ignore a world
 * At some point, it would be irrelevant since the money would be too low to be worth
 * as I would imagine some worlds would be more worthwhile than others
 * 
 * 
 */

/*
 * Submit description for Github about the Compute Shader stuff.
 * There's a few notes on what to do with the stuff.
Imported another project where I played with shaders and Compute Shaders with the goal to develop the technology to be used for the miner game within the Level Generation and Tiles Rendering systems.

The idea is to use a Compute Shader (CS) to generate a texture where each pixel is a tile, with the pixel's color representing the tile type.
Then, we would sample the texture by doing a World Space to Texture Space conversion and convert the color to a TileID.
We would also have a second texture to represent the Tiles that were dug up.

We'd have a script to do the World->Texture space conversion, texture lookup, color->TileID conversion, etc.

The TileID texture lookup wouldn't be super necessary on its own, we can easily compute TileID through C# on the fly.
The actual reason we need a TileIDs texture is for it to be passed into the Rendering pipeline.
Instead of having one quad for each tiles, we would have a single quad, the shader of which handles all the visual required.
Picking the proper texture to display for each TileIDs (should be an Atlas with all the tile types, matching TileIDs' ordering), hiding dug up tiles, rounding up corners around dug up tiles, perhaps even animating a tile eroding away as the player digs it up.

The TileIDs texture only need to contain what is visible on-screen. (it would probably be larger, for debug sake ?)
At depth 500, we don't need to know what's above 450, for instance. (unless we can see more than 50 lines, which we should not)
The Tiles Dug Up texture will require to contain all the tiles, all the time. Starts empty, then each new depth layer is more Y resolution. (or perhaps we only increase the size by X each X depth)

We still need to compute the collision. We only need the Tiles Dug Up texture for that.

We will probably be able to remove the ray intersection to fetch the tile, since this would be replaced with the aforementioned script to handle the conversions and lookups.
 */

/*
Coding Conventions

* PascalCase is used for classes and functions/methods
* camelCase is used for inspector variables
* SCREAMING_SNAKE_CASE is used for constants
* snake_case is used for everything else
*/
