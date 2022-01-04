using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileInfo", menuName = "NewTileInfo")]
public class TileInfo : ScriptableObject
{
    [Header("General Info")]
    public new string name;
    public Material material;

    [Space(10)]
    public bool addToInventory = true;
    [Tooltip("Weight in the Cargo.")]
    public float weight; // if weight or value is -1 or 0, do not store in cargo/inventory
    [Tooltip("Sell value in the store.")]
    public float value;

    /* Info to add and implement */
    // float Fuel Consumption (how much fuel to dig)
    // float Dig Time (how long to dig)
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
    // Each tile would be a line, it would be clamp from 0-1 in height,
    // and width would be depth


    public float GetWeight()
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

    public string GetName()
    {
        return name;
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
    public float GetSpawnPercent(int inputDepth)
    {
        int len = levelGenerationValues.Length;

        // If we're at the end, return the last percent
        if (GetDepth(len - 1) <= inputDepth)
            return GetPercent(len - 1);

        int index = len - 1;
        for (int i = 0; i < len; i++)
        {
            if (GetDepth(i) > inputDepth)
            {
                index = i;
                break;
            }
        }

        int previousDepth = GetDepth(index);
        int nextDepth = previousDepth;

        float previousPercent = GetPercent(index);
        float nextPercent = previousPercent;

        if (index != 0) // Get -1 if we're not 0.
        {
            previousDepth = GetDepth(index - 1);
            previousPercent = GetPercent(index - 1);
        }


        //float normal = Mathf.InverseLerp(previousDepth, nextDepth, inputDepth);
        //float percent = Mathf.Lerp(previousPercent, nextPercent, normal);

        float percent = previousPercent + (inputDepth - previousDepth) * (nextPercent - previousPercent) / (nextDepth - previousDepth);
        // This is equivalent, and should be faster.
        // If you get around to testing that,
        // I'd be curious to see the numbers.
        // This is something that will be evaluated very often.

        return percent;
    }
}
