using UnityEngine;

public enum TileType
{
    dirt,
    coal,
    iron,
    copper,
    rock,
    air
}

[CreateAssetMenu(fileName = "tileInfo_newTile", menuName = "New Tile Info")]
public class TileInfo : ScriptableObject
{
    [Header("General Info")]
    public TileType type;
    public Material material;

    public float digTime = 1.0f;
    public float fuelConsumption = 2.0f;
    public int minimumDrillLevel = 0; 

    [Space(10)]
    public bool addToInventory = true;
    [Tooltip("Weight in the Cargo.")]
    public int weight;
    [Tooltip("Sell value in the store.")]
    public int value;

    /* Info to add and implement */
    // int Dig Level (which drill is required to dig, for stuff like rocks. Most tiles will be 0)

    [Header("Procedural Generation Stuff")]
    [Range(0, 5)]
    [Tooltip("The size of the Perlin Noise.\nDo not use Integer Values.")]
    public float noiseSize = 1.01f;

    [System.Serializable]
    public struct LevelGenerationValues_
    {
        public int depth;
        [Range(0, 1)]
        public float percent; // aka threshold
    }
    [Tooltip("Make sure Depth is correctly ordered.")]
    public LevelGenerationValues_[] LevelGenerationValues;
    // It would be amazing to convert that system with a curve editor
    // Each tile would be a line, it would be clamped from 0-1 in height,
    // and width would be depth


    public int GetWeight()
    {
        return weight;
    }

    public float GetNoiseSize()
    {
        return noiseSize; // * 1.0013051412f; // Ensure it is not an integer.
    }

    public Material GetMaterial()
    {
        return material;
    }

    public new TileType GetType()
    {
        return type;
    }

    public float GetDigTime()
    {
        return digTime;
    }

    public int GetValue()
    {
        return value;
    }

    public float GetFuelConsumption()
    {
        return fuelConsumption;
    }

    // Only for convenience and readability
    private int GetDepth(int index)
    {
        return LevelGenerationValues[index].depth;
    }
    private float GetPercent(int index)
    {
        return LevelGenerationValues[index].percent;
    }

    // Go through the list of depth/percentage,
    // and fetch the interpolated percentage based on the depth.
    // This is essentially sampling from a multi-segment line graph.
    // Google around to see if there's a nicer way to do this !
    public float GetSpawnPercent(int input_depth)
    {
        int array_length = LevelGenerationValues.Length;

        //Debug.Log($"array length: {array_length} last_depth : {GetDepth(array_length - 1)} current depth : {input_depth}");

        // If we're at the end, return the last percent
        if (input_depth >= GetDepth(array_length - 1))
            return GetPercent(array_length - 1);


        int index = array_length - 1;
        for (int i = 0; i < array_length; i++)
        {
            if (GetDepth(i) > input_depth)
            {
                index = i;
                break;
            }
        }

        if (index <= 0) // is first line or earlier
            return GetPercent(0);


        int depth_0 = GetDepth(index - 1);
        float percent_0 = GetPercent(index - 1);

        int depth_1 = GetDepth(index);
        float percent_1 = GetPercent(index);


        //float normal = Mathf.InverseLerp(previousDepth, nextDepth, inputDepth);
        //float percent = Mathf.Lerp(previousPercent, nextPercent, normal);

        float percent = percent_0 + (input_depth - depth_0) * (percent_1 - percent_0) / (depth_1 - depth_0);
        // This is equivalent, and should be faster.
        // If you get around to testing that,
        // I'd be curious to see the numbers.
        // This is something that will be evaluated very often.

        return percent;
    }
}
