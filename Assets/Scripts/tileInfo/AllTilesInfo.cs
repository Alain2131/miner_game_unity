using UnityEngine;

//[CreateAssetMenu(fileName = "AllTiles", menuName = "AllTiles")]
public class AllTilesInfo : ScriptableObject
{
    [Header("Order defines spawn priority")]
    public TileInfo[] tiles;

    //public static AllTilesInfo Instance;

    // This runs even in the editor.. ?
    // Could that be an issue ?
    /*private void OnEnable()
    {
        Instance = this;
    }*/
}

/*
 * Process for adding a new Ore Type
 * 1) Create a New Tile Info, fill up the relevant info
 * 2) In the AllTilesInfo.cs script (this one), add a new entry for the tile,
 * and link it in the scriptable.
 * 3) In the UI, duplicate an OreInventorySlot_ object, with the subsequent tweaks to it
 * 
 * And that should be it !
 */
