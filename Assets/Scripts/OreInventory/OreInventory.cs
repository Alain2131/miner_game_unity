using System.Collections.Generic;
using UnityEngine;

public class OreInventory : MonoBehaviour
{
    [SerializeField] private OreInventoryUI oreInventoryUI;
    public AllTilesInfo allTilesInfo;

    [System.Serializable]
    public class OreEntry
    {
        public TileInfo tileInfo;
        public int amount;
    }

    [Tooltip("Public only for debugging purposes.")]
    public List<OreEntry> oresInCargo;

    void Start()
    {
        // Populate oresInCargo list. This feels a bit weird though
        oresInCargo = new List<OreEntry>(); // Remove any stuff from the UI - what does that mean ?
        foreach (TileInfo tile in allTilesInfo.GetAllTiles())
        {
            if (tile.addToInventory)
            {
                OreEntry newOre = new OreEntry();
                newOre.tileInfo = tile;
                newOre.amount = 0;
                oresInCargo.Add(newOre);
            }
        }
    }

    public void AddSingleOre(TileInfo tile)
    {
        // This is really not ideal for memory (same for the two Remove variants)
        // The only way to modify a struct is to copy it,
        // change the copy, and overwrite the original (I think)
        int ID = GetIDFromOre(tile.name);
        if(ID >= 0)
        {
            oresInCargo[ID].amount += 1;

            oreInventoryUI.UpdateUI();
        }
        else
        {
            Debug.LogError(tile.name + " not in the inventory !");
        }
    }

    public void RemoveSingleOre(TileInfo tile)
    {
        int ID = GetIDFromOre(tile.name);
        if (ID >= 0)
        {
            oresInCargo[ID].amount -= 1;

            oreInventoryUI.UpdateUI();
        }
    }

    public void RemoveAllOres()
    {
        for (int i = 0; i < oresInCargo.Count; i++)
        {
            oresInCargo[i].amount = 0;
        }

        oreInventoryUI.UpdateUI();
    }

    private int GetIDFromOre(string oreName)
    {
        for (int i = 0; i < oresInCargo.Count; i++)
        {
            if (oresInCargo[i].tileInfo.GetName() == oreName)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetOreCount(TileInfo tile)
    {
        int ID = GetIDFromOre(tile.name);
        return oresInCargo[ID].amount;
    }

    public int GetTotalValue() // used when selling ores
    {
        int total = 0;
        for (int i = 0; i < oresInCargo.Count; i++)
        {
            total += oresInCargo[i].tileInfo.value * oresInCargo[i].amount;
        }
        return total;
    }
}
