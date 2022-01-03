using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllTiles", menuName = "AllTiles")]
public class AllTilesInfo : ScriptableObject
{
    public TileInfo dirtTile;
    public TileInfo coalTile;
    public TileInfo airTile;

    public TileInfo GetDirt()
    {
        return dirtTile;
    }

    public TileInfo GetCoal()
    {
        return coalTile;
    }

    public TileInfo GetAir()
    {
        return airTile;
    }

    public TileInfo[] GetAllTiles()
    {
        return new TileInfo[] { coalTile, dirtTile };
    }
}
