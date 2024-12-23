using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public PlayerScript playerScript;
    public Money money;

    [Header("Player Upgrades")]
    public UpgradesManager upgradeManager;

    [Header("UI")]
    public OreInventory oreInventory;
    public GameObject oreInventoryUI;
    public GameObject upgradesStoreUI;
    public GameObject itemsStoreUI;
    public bool isUpgradeStoreOpen = false;
    public bool isItemsStoreOpen = false;

    [Header("Game Pause")]
    public GameObject pauseGameUI;
    public GameObject resumeButton;
    public bool gamePaused = false;

    [Header("Level Generation Stuff")]
    public int levelXSize = 100; // the width of the level
    public int levelYSize = 20; // how many lines exists at once. Might need to rename this
    public AllTilesInfo allTilesInfo;
    private TileInfo[] tiles_info; // Init'ed in Awake from allTilesObject


    public Controls controls;

    public static GameManager Instance;
    void Awake()
    {
        Instance = this;
        playerScript = player.GetComponent<PlayerScript>();
        tiles_info = allTilesInfo.GetAllTiles();

        pauseGameUI.SetActive(false);

        controls = new Controls();
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

    public bool ToggleUpgradeStoreUI()
    {
        isUpgradeStoreOpen = !upgradesStoreUI.activeSelf;
        upgradesStoreUI.SetActive(isUpgradeStoreOpen);

        if (isUpgradeStoreOpen)
        {
            controls.Gameplay.Disable();
            controls.MenuControls.Enable();
        }
        else
        {
            controls.Gameplay.Enable();
            controls.MenuControls.Disable();
        }

        return isUpgradeStoreOpen;
    }

    // Should probably make a function to combine with ToggleUpgradeStoreUI()
    // and simply take different arguments
    public bool ToggleItemsStoreUI()
    {
        isItemsStoreOpen = !itemsStoreUI.activeSelf;
        itemsStoreUI.SetActive(isItemsStoreOpen);

        if (isItemsStoreOpen)
        {
            controls.Gameplay.Disable();
            controls.MenuControls.Enable();
        }
        else
        {
            controls.Gameplay.Enable();
            controls.MenuControls.Disable();
        }

        return isItemsStoreOpen;
    }

    // Should probably be in a "level" class or something, so we can call it with gameManager.level.AddDugUpTile()
    public void AddDugUpTile(int pixel_ID)
    {
        tilesDugUp.Add(pixel_ID);
    }

    public bool IsTileDugUp(int pixel_ID)
    {
        // I wonder how fast that will be when the List will be thousands long
        bool is_dug = tilesDugUp.Contains(pixel_ID);
        bool is_air = PixelIDToTileInfo(pixel_ID).type == TileType.air; // I don't know how efficient doing the full algorithm is
        return is_dug || is_air;
    }

    public void DigTile(int pixel_ID)
    {
        if (IsTileDugUp(pixel_ID))
            return;

        TileScript ts = PixelIDToTileScript(pixel_ID);
        ts.SetEnabled(false); // Necessary as long as we have individual tiles.

        AddDugUpTile(pixel_ID); // Add the tile to the tilesDugUp List

        TileInfo tile_info = PixelIDToTileInfo(pixel_ID);

        oreInventory.AddSingleOre(tile_info);

        // Recompute Collision on the line
        GameObject line_object = ts.transform.parent.gameObject;
        line_object.GetComponent<LineGeneration>().RecomputeCollision(pixel_ID); // Only necessary when using the LevelGeneration's collision
    }




    // HORRENDOUS and expensive logic, but this should be temporary
    // Used to disable the tile object (to make it visually disappear)
    private TileScript PixelIDToTileScript(int pixel_ID)
    {
        // get all LineObjects
        // find matching Line ID
        int row = PixelIDY(pixel_ID);
        var line_objects = FindObjectsByType<LineGeneration>(FindObjectsSortMode.None);

        LineGeneration found = line_objects[0];
        foreach (var line_object in line_objects)
        {
            if(line_object.lineID == row)
            {
                found = line_object;
                break;
            }
        }

        // get children TileObjects
        // find matching pixelID
        foreach (Transform child in found.transform)
        {
            TileScript ts = child.GetComponent<TileScript>();
            if(ts.pixelID == pixel_ID)
            {
                return ts;
            }
        }

        Debug.LogError("TileScript not found :(");
        return null;
    }

















    public int PositionToPixelID(Vector3 position)
    {
        int idx = Mathf.FloorToInt(position.x);
        int idy = Mathf.FloorToInt(position.y);

        int pixel_ID = 99999;
        if (idy >= 0) // over ground
        {
            pixel_ID = idx + (idy * levelXSize) + levelXSize;
            pixel_ID *= -1;
        }
        else // under ground
        {
            pixel_ID = idx - (idy * levelXSize) - levelXSize;
        }

        return pixel_ID;
    }

    public Vector3 PixelIDToPosition(int pixel_ID)
    {
        // Need to handle negative pixelID properly
        // Decide on what -1 will be.
        // * Above 0
        // * Above 99
        if(pixel_ID < 0)
            return new Vector3(-1, -1, 0);

        
        int idx =  pixel_ID % levelXSize;
        int idy = -pixel_ID / levelXSize;
        //idy -= LevelYSize; // cancel out "one page offset"

        //float scale = GetMaterialScale();
        //float x_pos = (idx * scale) / resolution;
        //float y_pos = (idy * scale) / resolution;

        float tile_size = GetPixelWorldSize();
        float x_pos = idx * tile_size;
        float y_pos = idy * tile_size;
        y_pos -= 1;

        // x_pos and y_pos are at the bottom-left corner, so we add half the size
        x_pos += tile_size * 0.5f;
        y_pos += tile_size * 0.5f;

        return new Vector3(x_pos, y_pos, 0);
    }

    /*public Color SampleAtPosition(Vector3 position)
    {
        int pixel_ID = PositionToPixelID(position);

        Color Cd = SampleAtID(pixel_ID);
        return Cd;
    }*/

    // lateral position
    public int PixelIDX(int pixel_ID)
    {
        return pixel_ID % levelXSize;
    }

    // height
    public int PixelIDY(int pixel_ID)
    {
        return pixel_ID / levelXSize;
    }

    // This could probably be optimized with better math logic
    public int GetPixelIDAtOffset(int pixel_ID, int offsetx, int offsety)
    {
        int idx = PixelIDX(pixel_ID);
        int idy = PixelIDY(pixel_ID);

        idx = Mathf.Abs(idx);

        idx += offsetx;
        idy += offsety;



        //Debug.Log("---------------");
        //Debug.Log(idx);
        //Debug.Log(idy);


        // Sampling Out of Bounds
        if (idx < 0 || idy < 0 || idx > levelXSize) // || idy > levelXSize <- caused an issue at one point, leaving for reference, will delete later
        {
            //Debug.LogWarning($"PixelID offset out of bound, {pixel_ID} {offsetx} {offsety}");
            return -1;
        }

        int final_pixel_ID = idx + (idy * levelXSize);
        return final_pixel_ID;
    }

    // Basically, the world generation algorithm
    public TileInfo PixelIDToTileInfo(int pixel_ID, float seed = 0f)
    {
        int level_column = PixelIDX(pixel_ID);
        int level_row = PixelIDY(pixel_ID); // aka depth

        float noise_value, noise_size, current_depth_spawn_percent;
        const float BIAS = 0.001f; // smol bias to remove 0 in noise_value

        // Loop through all TileInfos, and place the first "valid" tile
        // Essentially, we give priority to the first types of tile.
        // See allTilesInfo.GetAllTiles() for the order.
        foreach (TileInfo tile_info in tiles_info)
        {
            noise_size = tile_info.GetNoiseSize();
            current_depth_spawn_percent = tile_info.GetSpawnPercent(level_row);

            if (current_depth_spawn_percent == 0)
                continue;

            noise_value = Mathf.PerlinNoise(level_column * noise_size + seed, level_row * noise_size + seed);
            noise_value = Mathf.Clamp01(noise_value + BIAS);

            /* // debug in case something goes wrong
            if (pixel_ID == 2128)
            {
                Debug.Log($"tile {tile_info.type}, column {level_column}, row {level_row}, noise_size {noise_size}, spawn_percent {current_depth_spawn_percent}, noise {noise_value}");
            }//*/

            if (noise_value <= current_depth_spawn_percent)
                return tile_info;
        }

        Debug.LogError($"NULL TileInfo for pixel_ID {pixel_ID} ! Returning air instead.");
        return tiles_info[0]; // air
    }

    // This logic could be changed to calculate tileWorldSize on Awake()
    // Then just return that value
    // I'm leaving that in as a special sauce example,
    // no idea if this Nullable value method is bad practice
    public float GetPixelWorldSize()
    {
        return 1.0f;
    }

    // Debug Feature
    public void CreateQuadAtPixelID(int pixel_ID)
    {
        float size = GetPixelWorldSize();
        Vector3 position = PixelIDToPosition(pixel_ID);

        GameObject square = GameObject.CreatePrimitive(PrimitiveType.Quad);
        square.transform.localScale = new Vector3(size, size, size);
        square.transform.position = position;
    }



































    public void TogglePauseGame()
    {
        // HACK - exit Upgrades UI menu with Escape
        // This is a bad band-aid, we need a better way
        // to close the Upgrades UI (and others, eventually) when in a menu.
        if (!gamePaused && isUpgradeStoreOpen)
        {
            ToggleUpgradeStoreUI();
            return;
        }
        if (!gamePaused && isItemsStoreOpen)
        {
            ToggleItemsStoreUI();
            return;
        }

        gamePaused = !gamePaused;
        pauseGameUI.SetActive(gamePaused);

        // stop player movement
        // velocity is automatically stored/restored, yay !
        if (gamePaused)
        {
            playerScript.rb.simulated = false;

            controls.Gameplay.Disable();
            controls.MenuControls.Enable();
        }
        else
        {
            playerScript.rb.simulated = true;

            controls.Gameplay.Enable();
            controls.MenuControls.Disable();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    // This will be saved at some point, so we'll need a constructor here that loads tilesDugUp when the game starts
    // Note that it is a list, and we can only save arrays. We'll have to convert 'em up
    // https://www.youtube.com/watch?v=XOjd_qU2Ido // save/load, from Brackeys
}
