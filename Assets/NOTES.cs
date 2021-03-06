/* What needs to be done in the game */
/* The list is in no particular order */

// Note :
// When I say "DONE", I mean that it's working,
// but it may be very janky.

/* 
 * Do the Fuel and Hull (with consumption and damage) (DONE)
 * Then do the Refuel and Hull repair buildings (DONE)
 * Then do the Upgrades building (will require UI)
 * Then start to work on the item upgrades (drill, fuel tank, hull, cargo size)
 */

// Core Mechanics
// - Implement Digging (DONE)
// - * Add tile to inventory (DONE)
// - * Move Player to the center of the tile (DONE)
// - * * Add jitter animation to the pod, and maybe the camera as well (DONE)
// - * * Disable player movement input during the animation (DONE)
// - Inventory
// - * Allow possibility of discarding ores (to lower weight) (DONE)
// - * Show ore value/weight
// - Items
// - Money (DONE)
// - Upgrades
// - * Drill Speed
// - * Movement Speed
// - * Fuel tank
// - * Cargo size
// - * Hull life
// - Fuel consumption (DONE)
// - Hull life
// - * Fall damage (DONE)
// - * Dying/respawning (for fuel as well)
// - * * Steep money malus when dying (remove half or something)
// - * * Remove some amount of ore in the cargo (some or all of it)
// - * * Fill fuel
// - Stores
// - * Upgrades (buying/selling)
// - * Items (dynamite, teleport, etc)
// - * Ore selling (DONE)
// - * Fuel buying
// - * Hull repair
// - Level saving
// - Level generation (DONE)
// - * Perlin Noise Implementation (DONE)
// - Pod weight based on ores in cargo bay
// - * Movement become sluggish, flying is slower or not possible anymore

// UI
// - Inventory (DONE)
// - Items (could be a part of the inventory screen)
// - Stores
// - Fuel/life (DONE)
// - Money (DONE)
// - Menus
// - * Main Menu
// - * Pause
// - * Options
// - Depth gauge
// - Dialogs ?
// - * Animate text apparition
// - * * Skip animation on click
// - * Go back to previous dialog box
// - * Witty stuff
// - * * If we go back too much, some NPCs could get annoyed that they need to repeat themselves
// - * * If we skip too much, some NPCs could get annoyed that we don't listen to them
// - * * * Annoyance could be shown with a smaller more subtle dialog box, where the NPC grumbles a bit

// Misc
// - Add level bound, left and right (DONE)
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

// Unity Project stuff
// https://www.youtube.com/watch?v=k3wHqPZUldw
// Assembly Definition
// Splits the code we write into different .dll to speed up compiles on reload
// Good info about .dll as well

// https://www.youtube.com/watch?v=p7GfSsQvR78
// Store data in text files and load it up

// https://learn.unity.com/tutorial/optimizing-unity-ui#
// UI optimisation
