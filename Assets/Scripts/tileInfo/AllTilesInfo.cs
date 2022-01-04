using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllTiles", menuName = "AllTiles")]
public class AllTilesInfo : ScriptableObject
{
    public TileInfo dirtTile;
    public TileInfo coalTile;
    public TileInfo ironTile;
    public TileInfo airTile;

    public TileInfo GetAir()
    {
        return airTile;
    }

    public TileInfo[] GetAllTiles()
    {
        return new TileInfo[] { ironTile, coalTile, dirtTile };
    }
}
