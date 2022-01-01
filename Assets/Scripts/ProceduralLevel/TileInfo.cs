using UnityEngine;

public enum TileTypes
{
    dirt,
    coal
};

public class TileInfo// : MonoBehaviour
{
    TileTypes type;
    string name;
    Material material;
    float value; // unused
    float weight; // unused

    // They might not need to be float
    readonly float[] values = new float[]{ 0f, 20f };
    readonly float[] weights = new float[] { 0f, 100f };


    public TileInfo(TileTypes type)
    {
        this.type = type;
        this.name = type.ToString();
        this.material = Resources.Load("Materials/" + name + "Mat", typeof(Material)) as Material;

        int ID = (int)type;
        this.value = values[ID];
        this.weight = weights[ID];
    }

    public TileTypes Type()
    {
        return type;
    }

    public string Name()
    {
        return name;
    }

    public Material Material()
    {
        return material;
    }

    public float Value()
    {
        return value;
    }

    public float Weight()
    {
        return weight;
    }
}

public class AllTiles
{
    static private TileInfo DirtTile = new TileInfo(TileTypes.dirt);
    static private TileInfo CoalTile = new TileInfo(TileTypes.coal);

    public static TileInfo Dirt()
    {
        return DirtTile;
    }

    public static TileInfo Coal()
    {
        return CoalTile;
    }
}
