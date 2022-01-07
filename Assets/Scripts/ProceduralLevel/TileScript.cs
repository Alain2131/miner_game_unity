using UnityEngine;

/*
 * The level generation places a tileObject,
 * which in turn references this script.
 */

[RequireComponent(typeof(MeshRenderer), typeof(BoxCollider2D))]
public class TileScript : MonoBehaviour
{
    public TileInfo tileInfo;
    public int x_ID; // the lateral position on the line, including air tiles
    public int uniqueID = -1; // lineID * xSize + x_ID, a unique ID for every single tiles to store which tiles have been dug up

    public void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            transform.gameObject.SetActive(true);
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
    }

    public void SetMaterial(Material mat)
    {
        GetComponent<Renderer>().material = mat;
    }

    public void DigTile()
    {
        SetEnabled(false);

        GameManager gameManager = GameManager.Instance;
        gameManager.Add(uniqueID); // add the tile to the tilesDugUp List

        OreInventory oreInventory = OreInventory.Instance;
        if(tileInfo.addToInventory)
            oreInventory.Add(tileInfo); // increase the tile count in the inventory (if applicable)

        // Recompute Collision on the line
        GameObject parent = transform.parent.gameObject;
        parent.GetComponent<LineGeneration>().RecomputeCollision(x_ID);
    }
}
