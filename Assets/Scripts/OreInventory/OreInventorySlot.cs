using UnityEngine;
using UnityEngine.UI;

/*
 * If the inventory is opened, and the player digs a tile, call UpdateUI()
 * If the inventory is opened, call UpdateUI()
 * If the player discards any ore, call UpdateUI()
 * 
 * If the inventory is closed, do nothing. Even when selling ores.
 */
public class OreInventorySlot : MonoBehaviour
{
    public Text oreName;
    public Text oreQty;

    public TileInfo tileInfo;

    private OreInventory oreInventory;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        // We can either set the name in the level, or automatically
        // Problem with setting the name in the level is the prefabs
        // If we modify one and propagate the changes,
        // the naming will be discarded.
        // But it is nice to see the actual result in the engine
        // rather than having to start the game every time.

        //oreName.text = tileInfo.name;
    }

    public void UpdateSlotUI()
    {
        if (!oreInventory)
            oreInventory = GameManager.Instance.oreInventory;

        int count = oreInventory.GetOreCount(tileInfo);
        oreQty.text = count.ToString();
    }

    public void RemoveItem()
    {
        oreInventory.RemoveSingleOre(tileInfo);
    }
}
