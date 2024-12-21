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
    public GameObject upgradesStore_UI;
    public GameObject itemsStore_UI;
    public bool isUpgradeStoreOpen = false;
    public bool isItemsStoreOpen = false;

    [Header("Game Pause")]
    public GameObject pauseGameUI;
    public GameObject resumeButton;
    public bool gamePaused = false;

    [Header("Level Generation Stuff")]
    public int LevelXSize = 100; // the width of the level
    public int LevelYSize = 20; // how many lines exists at once. Might need to rename this


    public Controls controls;

    public static GameManager Instance;
    void Awake()
    {
        Instance = this;
        playerScript = player.GetComponent<PlayerScript>();
        
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
        isUpgradeStoreOpen = !upgradesStore_UI.activeSelf;
        upgradesStore_UI.SetActive(isUpgradeStoreOpen);

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
        isItemsStoreOpen = !itemsStore_UI.activeSelf;
        itemsStore_UI.SetActive(isItemsStoreOpen);

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
    public void AddDugUpTile(int tileID)
    {
        tilesDugUp.Add(tileID);
    }

















    public int PositionToPixelID(Vector3 position)
    {
        // Needs to handle positive Y

        int idx = Mathf.FloorToInt(position.x);
        int idy = -Mathf.FloorToInt(position.y);

        idx = Mathf.Clamp(idx, 0, LevelXSize - 1);
        idy = Mathf.Clamp(idy, 0, LevelXSize - 1);

        int pixelID = idx + (idy * LevelXSize) - LevelXSize;
        return pixelID;
    }

    public Vector3 PixelIDToPosition(int pixelID)
    {
        // Need to handle negative pixelID properly
        // Decide on what -1 will be.
        // * Above 0
        // * Above 99
        if(pixelID < 0)
            return new Vector3(-1, -1, 0);

        
        int idx = pixelID % LevelXSize;
        int idy = -pixelID / LevelXSize;
        //idy -= LevelYSize; // cancel out "one page offset"

        //float scale = GetMaterialScale();
        //float xPos = (idx * scale) / resolution;
        //float yPos = (idy * scale) / resolution;

        float tileSize = GetPixelWorldSize();
        float xPos = idx * tileSize;
        float yPos = idy * tileSize;
        yPos -= 1;

        // xPos and yPos at the bottom-left corner, so we add half the size
        xPos += tileSize * 0.5f;
        yPos += tileSize * 0.5f;

        return new Vector3(xPos, yPos, 0);
    }

    /*public Color SampleAtPosition(Vector3 position)
    {
        int pixelID = PositionToPixelID(position);

        Color Cd = SampleAtID(pixelID);
        return Cd;
    }*/

    // This could probably be optimized with better math logic
    public int GetPixelAtOffset(int pixelID, int offsetX, int offsetY)
    {
        int idx = pixelID % LevelXSize;
        int idy = pixelID / LevelXSize;
        
        idx = Mathf.Abs(idx);

        idx += offsetX;
        idy += offsetY;



        Debug.Log("---------------");
        Debug.Log(idx);
        Debug.Log(idy);


        // Sampling is Out of Bounds
        if (idx < 0 || idy < 0 || idx > LevelXSize || idy > LevelXSize)
            return -1;

        int finalPixelID = idx + (idy * LevelXSize);
        return finalPixelID;
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
    public void CreateQuadAtPixelID(int pixelID)
    {
        float size = GetPixelWorldSize();
        Vector3 position = PixelIDToPosition(pixelID);

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
