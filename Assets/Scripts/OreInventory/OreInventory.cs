using System.Collections.Generic;
using UnityEngine;

public class OreInventory : MonoBehaviour
{
    public static OreInventory Instance;

    [SerializeField] private OreInventoryUI oreInventoryUI;

    [System.Serializable]
    public struct OreEntry
    {
        public TileInfo tileInfo;
        public int amount;
    }
    [Tooltip("Public only for debugging purposes.")]
    public List<OreEntry> oresInCargo;

    private void Awake()
    {
        Instance = this;

        oresInCargo = new List<OreEntry>(); // Remove any stuff from the UI
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

    void Start()
    {
        //oreInventoryUI = OreInventoryUI.Instance;
    }

    public void AddSingleOre(TileInfo tile)
    {
        // This is really not ideal for memory (same for the two Remove variants)
        // The only way to modify a struct is to copy it,
        // change the copy, and overwrite the original (I think)
        int ID = GetIDFromOre(tile.name);
        if(ID >= 0)
        {
            OreEntry editedOre = oresInCargo[ID];
            editedOre.amount += 1;
            oresInCargo[ID] = editedOre;

            UpdateUIIfOpened();
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

            UpdateUIIfOpened();
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

        UpdateUIIfOpened();
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

    private void UpdateUIIfOpened()
    {
        // If we dig an ore while the inventory UI is opened, update it.
        if (oreInventoryUI.IsOpen())
        {
            oreInventoryUI.UpdateUI();
        }
    }
}
