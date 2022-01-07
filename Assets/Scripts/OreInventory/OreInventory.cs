using System.Collections.Generic;
using UnityEngine;

public class OreInventory : MonoBehaviour
{
    public GameManager gameManager;
    public static OreInventory Instance;

    private OreInventoryUI oreInventoryUI;

    [System.Serializable]
    public struct OreEntry
    {
        public TileInfo OreInfo;
        public int      OreAmount;
    }
    public List<OreEntry> OresInCargo;

    private void Awake()
    {
        Instance = this;
        gameManager = GetComponent<GameManager>();

        // We might want to make sure this happened for sure
        // And ensure that we don't add duplicates (that we might have set manually in the inspector for debugging)
        foreach (TileInfo tile in gameManager.allTilesInfo.GetAllTiles())
        {
            if (tile.addToInventory)
            {
                OreEntry newOre = new OreEntry();
                newOre.OreInfo = tile;
                newOre.OreAmount = 0;
                OresInCargo.Add(newOre);
            }
        }
    }

    void Start()
    {
        oreInventoryUI = OreInventoryUI.Instance;
    }

    public void Add(TileInfo tile)
    {
        // This is really not ideal for memory (same for the two Remove variants)
        // The only way to modify a struct is to copy it,
        // change the copy, and overwrite the original (I think)
        int ID = GetIDFromOre(tile.name);
        if(ID >= 0)
        {
            OreEntry editedOre = OresInCargo[ID];
            editedOre.OreAmount += 1;
            OresInCargo[ID] = editedOre;

            UpdateUIIfOpened();
        }
        else
        {
            Debug.LogError(tile.name + " not in the inventory !");
        }
    }

    public void Remove(TileInfo tile)
    {
        int ID = GetIDFromOre(tile.name);
        if (ID >= 0)
        {
            OreEntry editedOre = OresInCargo[ID];
            editedOre.OreAmount -= 1;
            if (editedOre.OreAmount < 0)
                return;
            OresInCargo[ID] = editedOre;

            UpdateUIIfOpened();
        }
    }

    public void RemoveAll()
    {
        for (int i = 0; i < OresInCargo.Count; i++)
        {
            OreEntry editedOre = OresInCargo[i];
            editedOre.OreAmount = 0;
            OresInCargo[i] = editedOre;
        }

        UpdateUIIfOpened();
    }

    private int GetIDFromOre(string oreName)
    {
        for (int i = 0; i < OresInCargo.Count; i++)
        {
            if (OresInCargo[i].OreInfo.GetName() == oreName)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetOreCount(TileInfo tile)
    {
        int ID = GetIDFromOre(tile.name);
        return OresInCargo[ID].OreAmount;
    }

    public int GetTotalValue() // used when selling ores
    {
        int total = 0;
        for (int i = 0; i < OresInCargo.Count; i++)
        {
            total += OresInCargo[i].OreInfo.value * OresInCargo[i].OreAmount;
        }
        return total;
    }

    private void UpdateUIIfOpened()
    {
        // If we dig an ore while the inventory UI is opened, update it.
        if (oreInventoryUI.IsOpen())
        {
            oreInventoryUI.UpdateUI();
        }
    }
}
