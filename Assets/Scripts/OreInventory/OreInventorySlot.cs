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

    private void Start()
    {
        oreName.text = tileInfo.name;
    }

    public void UpdateSlotUI()
    {
        if (!oreInventory)
            oreInventory = OreInventory.Instance;

        int count = oreInventory.GetOreCount(tileInfo);
        oreQty.text = count.ToString();
    }

    public void RemoveItem()
    {
        //Debug.Log("Remove Item !");
        oreInventory.Remove(tileInfo);
    }
}
