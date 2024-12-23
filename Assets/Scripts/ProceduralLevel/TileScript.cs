using UnityEngine;

/*
 * The level generation places a tileObject,
 * which in turn references this script.
 */

[RequireComponent(typeof(MeshRenderer), typeof(BoxCollider2D))]
public class TileScript : MonoBehaviour
{
    // lineID * xSize + x_ID, a unique ID for every single tiles to store which tiles have been dug up
    // coincidendally matches up with Pixel ID, should probably only calculate once and share the info
    public int pixelID = -1;

    public void SetEnabled(bool enabled)
    {
        transform.gameObject.SetActive(enabled);
    }

    public void SetMaterial(Material mat)
    {
        GetComponent<Renderer>().material = mat;
    }
}
