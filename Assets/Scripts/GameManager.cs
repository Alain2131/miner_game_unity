using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public PlayerScript playerScript;
    public Money money;
    public OreInventory oreInventory;

    [Header("Player Upgrades")]
    public UpgradesManager upgradeManager;

    [Header("Level Generation Stuff")]
    public int LevelXSize = 100; // the width of the level
    public int LevelYSize = 20; // how many lines exists at once. Might need to rename this

    public static GameManager Instance;
    void Awake()
    {
        Instance = this;
        playerScript = player.GetComponent<PlayerScript>();
    }

    // A flat integer List of all the tiles that were dug up.
    // The data is stored like so
    // lineID * xSize + x_ID
    // in words, the current depth multiplied by the amount of tiles in X, plus the current ID on the line
    // we multiply it so we have a unique ID for every tile
    //[HideInInspector]
    [Space(5)]
    [Tooltip("Public only for debugging purposes.")]
    public List<int> tilesDugUp = new List<int>();
    // The TilesDugUp stuff could technically be
    // somewhere other than inside GameManager.cs,
    // but still in a script on the GameManager Object.

    // Should probably be in a "level" class or something, so we can call it with gameManager.level.AddDugUpTile()
    public void AddDugUpTile(int tileID)
    {
        tilesDugUp.Add(tileID);
    }

    // This will be saved at some point, so we'll need a constructor here that loads tilesDugUp when the game starts
    // Note that it is a list, and we can only save arrays. We'll have to convert 'em up
    // https://www.youtube.com/watch?v=XOjd_qU2Ido // save/load, from Brackeys
}
