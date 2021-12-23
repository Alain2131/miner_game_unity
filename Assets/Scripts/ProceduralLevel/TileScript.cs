using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(BoxCollider2D))]
public class TileScript : MonoBehaviour
{
    /*
    public enum TileType
    {
        Dirt, Coal, Iron
    }
    */

    public int x_ID;
    //public TileType tileType;

    public void SetMaterial(string tileType)
    {
        Material tileMat = Resources.Load("Materials/" + tileType + "Mat", typeof(Material)) as Material;
        GetComponent<Renderer>().material = tileMat;
    }

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

    public void DigTile()
    {
        SetEnabled(false);

        // Recompute Collision
        GameObject parent = transform.parent.gameObject;
        parent.GetComponent<LineGeneration>().RecomputeCollision(x_ID);
    }
}
