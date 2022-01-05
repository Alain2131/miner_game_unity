using System.Collections.Generic;
using UnityEngine;

public class OreInventory : MonoBehaviour
{
    private GameManager gameManager;
    public static OreInventory Instance;

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
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

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

    public void Add(string oreName)
    {
        // This is really not ideal for memory (same for the two Remove variants)
        // The only way to modify a struct is to copy it,
        // change the copy, and overwrite the original (I think)
        int ID = GetIDFromOre(oreName);
        if(ID >= 0)
        {
            OreEntry editedOre = OresInCargo[ID];
            editedOre.OreAmount += 1;
            OresInCargo[ID] = editedOre;
        }
        else
        {
            Debug.LogError(oreName + " not in the inventory !");
        }
    }

    public void Remove(string oreName)
    {
        int ID = GetIDFromOre(oreName);
        if (ID >= 0)
        {
            OreEntry editedOre = OresInCargo[ID];
            editedOre.OreAmount -= 1;
            OresInCargo[ID] = editedOre;
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
}
