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
    public Text oreValue;
    public Text oreName;
    public Text oreQty;
    public Text oreWeight;

    public TileInfo tileInfo;

    //private OreInventory oreInventory;
    private GameManager game_manager;

    private void Awake()
    {
        game_manager = GameManager.Instance;
        //oreInventory = gameManager.oreInventory;
    }

    private void Start()
    {
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
        //Debug.Log(oreInventory);
        /*if (!oreInventory)
            oreInventory = gameManager.oreInventory;*/

        int count = game_manager.oreInventory.GetOreCount(tileInfo);
        oreQty.text = count.ToString();

        float total_weight = tileInfo.weight * count;
        oreWeight.text = total_weight.ToString() + "KG";
        oreValue.text = "$" + tileInfo.value.ToString();
    }

    public void RemoveItem()
    {
        game_manager.oreInventory.RemoveSingleOre(tileInfo);
    }
}
