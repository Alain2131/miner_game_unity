public class Building_SellOres : Interactable
{
    private OreInventory oreInventory;

    private void Start()
    {
        oreInventory = OreInventory.Instance;
    }
    
    public override void Interact()
    {
        // Sell all Ores in the OreInventory
        // Again, this is janky because we have to create a newOre variable,
        // modify it, and then re-assign it in the array,
        // all because of the OreEntry Struct.
        int total = 0;
        for(int i=0; i<oreInventory.OresInCargo.Count; i++)
        {
            OreInventory.OreEntry newOre = oreInventory.OresInCargo[i];

            int count = newOre.OreAmount;
            int value = GetTileValue(newOre.OreName);
            total += count * value;

            // Reset Amount
            newOre.OreAmount = 0;
            oreInventory.OresInCargo[i] = newOre;
        }

        Money.Instance.Sell(total);
    }

    private int GetTileValue(string tileName)
    {
        // This is janky
        // A better solution might be to directly store the TileInfo
        // in the OreInventory instead of the TileName
        TileInfo[] tiles = GameManager.Instance.allTilesInfo.GetAllTiles();
        foreach(TileInfo tile in tiles)
        {
            if(tile.name == tileName)
            {
                return (int)tile.value;
            }
        }
        return -1;
    }
}
