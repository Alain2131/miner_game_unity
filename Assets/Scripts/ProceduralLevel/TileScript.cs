using UnityEngine;

/*
 * The level generation places a tileObject,
 * which in turn references this script.
 */

[RequireComponent(typeof(MeshRenderer), typeof(BoxCollider2D))]
public class TileScript : MonoBehaviour
{
    public TileInfo tileInfo;
    public int xID; // the lateral position on the line, including air tiles

    // lineID * xSize + x_ID, a unique ID for every single tiles to store which tiles have been dug up
    // coincidendally matches up with Pixel ID, should probably only calculate once and share the info
    public int uniqueID = -1;


    private GameManager game_manager;
    private OreInventory ore_inventory;

    private void Start()
    {
        game_manager = GameManager.Instance;
        ore_inventory = game_manager.oreInventory;
    }

    public void SetEnabled(bool enabled)
    {
        transform.gameObject.SetActive(enabled);
    }

    public void SetMaterial(Material mat)
    {
        GetComponent<Renderer>().material = mat;
    }

    public void DigTile()
    {
        SetEnabled(false);

        game_manager.AddDugUpTile(uniqueID); // add the tile to the tilesDugUp List

        if(tileInfo.addToInventory)
            ore_inventory.AddSingleOre(tileInfo); // increase the tile count in the inventory (if applicable)

        // Recompute Collision on the line
        GameObject line_object = transform.parent.gameObject;
        line_object.GetComponent<LineGeneration>().RecomputeCollision(xID);
    }
}
