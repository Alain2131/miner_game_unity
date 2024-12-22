using UnityEngine;

public enum TileType
{
    dirt,
    coal,
    iron,
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
    public struct LevelGenerationValues
    {
        public int depth;
        [Range(0, 1)]
        public float percent; // aka threshold
    }
    [Tooltip("Make sure Depth is correctly ordered.")]
    public LevelGenerationValues[] levelGenerationValues;
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
        return levelGenerationValues[index].depth;
    }
    private float GetPercent(int index)
    {
        return levelGenerationValues[index].percent;
    }

    // Go through the list of depth/percentage
    // fetch the interpolated percentage based on the depth
    public float GetSpawnPercent(int input_depth)
    {
        int len = levelGenerationValues.Length;

        // If we're at the end, return the last percent
        if (GetDepth(len - 1) <= input_depth)
            return GetPercent(len - 1);

        int index = len - 1;
        for (int i = 0; i < len; i++)
        {
            if (GetDepth(i) > input_depth)
            {
                index = i;
                break;
            }
        }

        int previous_depth = GetDepth(index);
        int next_depth = previous_depth;

        float previous_percent = GetPercent(index);
        float next_percent = previous_percent;

        if (index != 0) // Get -1 if we're not 0.
        {
            previous_depth = GetDepth(index - 1);
            previous_percent = GetPercent(index - 1);
        }


        //float normal = Mathf.InverseLerp(previousDepth, nextDepth, inputDepth);
        //float percent = Mathf.Lerp(previousPercent, nextPercent, normal);

        float percent = previous_percent + (input_depth - previous_depth) * (next_percent - previous_percent) / (next_depth - previous_depth);
        // This is equivalent, and should be faster.
        // If you get around to testing that,
        // I'd be curious to see the numbers.
        // This is something that will be evaluated very often.

        return percent;
    }
}
