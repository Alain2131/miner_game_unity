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
