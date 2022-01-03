using UnityEngine;

/*
 * The level generation places a tileObject,
 * which in turn references this script.
 */

[RequireComponent(typeof(MeshRenderer), typeof(BoxCollider2D))]
public class TileScript : MonoBehaviour
{
    public int x_ID; // the lateral position on the line, including air tiles

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

        // Recompute Collision
        GameObject parent = transform.parent.gameObject;
        parent.GetComponent<LineGeneration>().RecomputeCollision(x_ID);
    }
}
