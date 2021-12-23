using UnityEngine;
//using System.Collections.Generic;

// I need to store the properties of the tiles
[RequireComponent(typeof(MeshRenderer))]
public class tile : MonoBehaviour
{
    public enum TileType
    {
        Dirt, Coal, Iron
    }

    public int ID;
    public TileType tileType;

    public void SetMaterial(string tileType)
    {
        Material tileMat = Resources.Load("Materials/" + tileType + "Mat", typeof(Material)) as Material;
        GetComponent<Renderer>().material = tileMat;
    }

    public void DisableTile()
    {
        transform.gameObject.SetActive(false);
        //GetComponent<MeshRenderer>().enabled = false;
        //GetComponent<BoxCollider2D>().enabled = false;
        //GetComponent<tile>().enabled = false;
    }

    public void EnableTile()
    {
        transform.gameObject.SetActive(true);
        //GetComponent<MeshRenderer>().enabled = true;
        //GetComponent<BoxCollider2D>().enabled = true;
        //GetComponent<tile>().enabled = true;
    }
}
