using System.Collections.Generic;
using UnityEngine;

public class OreInventory : MonoBehaviour
{
    [SerializeField] private OreInventoryUI oreInventoryUI;

    // Looks like this is evil. "Mutable" means that it can change.
    // https://stackoverflow.com/questions/441309/why-are-mutable-structs-evil
    // I'll probably have to change how OreEntry is represented
    [System.Serializable]
    public struct OreEntry
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
        foreach (TileInfo tile in AllTilesInfo.Instance.GetAllTiles())
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
            // Looks like this is evil
            // https://stackoverflow.com/questions/441309/why-are-mutable-structs-evil
            OreEntry editedOre = oresInCargo[ID];
            editedOre.amount += 1;
            oresInCargo[ID] = editedOre;

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
            OreEntry editedOre = oresInCargo[ID];
            editedOre.amount -= 1;
            if (editedOre.amount < 0)
                return;
            oresInCargo[ID] = editedOre;

            oreInventoryUI.UpdateUI();
        }
    }

    public void RemoveAllOres()
    {
        for (int i = 0; i < oresInCargo.Count; i++)
        {
            OreEntry editedOre = oresInCargo[i];
            editedOre.amount = 0;
            oresInCargo[i] = editedOre;
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
