using System.Collections.Generic;
using UnityEngine;

public class OreInventory : MonoBehaviour
{
    public AllTilesInfo allTilesInfo;

    public Transform itemsParent;
    public GameObject oreInventoryUI;

    private OreInventory oreInventory;
    private OreInventorySlot[] slots;

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


        // UI stuff
        oreInventory = GameManager.Instance.oreInventory;

        // We could automatically populate the slots,
        // but I prefer adding them manually in the Level.
        slots = itemsParent.GetComponentsInChildren<OreInventorySlot>();

        OreInventorySlotsSanityCheck();

        // Make sure the inventory is closed when the game starts
        oreInventoryUI.SetActive(false);
    }


    public void AddSingleOre(TileInfo tile)
    {
        int ID = GetIDFromOre(tile.type);
        if(ID >= 0)
        {
            int totalWeight = GetOreWeight(tile);
            int maxWeight = GameManager.Instance.playerScript.cargoSize; // it's not great to have to access the player here
            int newTotalWeight = totalWeight + tile.weight;
            if (maxWeight < newTotalWeight)
            {
                Debug.LogError("Can't add ore due to full cargo.");
                return;
            }

            oresInCargo[ID].amount += 1;
            UpdateUI();

            if (maxWeight == newTotalWeight)
            {
                // Need to have some UI instead
                Debug.LogWarning("Cargo full !");
            }
            // Warning debug if almost full at a percentage of total size
            else if (maxWeight*0.8f <= newTotalWeight)
            {
                // Need to have some UI instead
                Debug.LogWarning("Cargo almost full !");
            }
        }
        else
        {
            Debug.LogError(tile.name + " not in the inventory !");
            return;
        }
    }

    public void RemoveSingleOre(TileInfo tile)
    {
        int ID = GetIDFromOre(tile.type);
        if (ID >= 0)
        {
            if (oresInCargo[ID].amount == 0)
                return;

            oresInCargo[ID].amount -= 1;
            UpdateUI();
        }
    }

    public void RemoveAllOres()
    {
        for (int i = 0; i < oresInCargo.Count; i++)
        {
            oresInCargo[i].amount = 0;
        }

        UpdateUI();
    }

    private int GetIDFromOre(TileType oreName)
    {
        for (int i = 0; i < oresInCargo.Count; i++)
        {
            if (oresInCargo[i].tileInfo.GetType() == oreName)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetOreCount(TileInfo tile)
    {
        int ID = GetIDFromOre(tile.type);
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

    public int GetOreWeight(TileInfo tile)
    {
        int ID = GetIDFromOre(tile.type);
        if (ID >= 0)
        {
            return oresInCargo[ID].amount * tile.weight;
        }

        return -1;
    }

    // UI stuff
    public bool ToggleInventoryUI()
    {
        oreInventoryUI.SetActive(!oreInventoryUI.activeSelf);

        if(oreInventoryUI.activeSelf)
            UpdateUI();

        return !oreInventoryUI.activeSelf;
    }

    public void UpdateUI()
    {
        if (IsOpen())
        {
            foreach (OreInventorySlot slot in slots)
            {
                slot.UpdateSlotUI();
            }
        }
    }

    public bool IsOpen()
    {
        return oreInventoryUI.activeSelf;
    }

    private void OreInventorySlotsSanityCheck()
    {
        // Make sure all ores have a slot
        // This is more for debugging than anything
        bool found;
        foreach (OreInventory.OreEntry entry in oreInventory.oresInCargo)
        {
            found = false;
            foreach (OreInventorySlot slot in slots)
            {
                if (entry.tileInfo == slot.tileInfo)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Debug.LogError("Missing UI Inventory Slot for " + entry.tileInfo.name);
            }
        }
    }
}
