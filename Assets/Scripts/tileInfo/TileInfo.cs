using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewTileInfo", menuName = "NewTileInfo")]
public class TileInfo : ScriptableObject
{
    public new string name;

    public Material material;

    public float weight; // weight in cargo
    public float value; // sell value in store

    // Separator

    [Serializable]
    public struct LevelGenerationValues
    {
        public int depth;
        public float percent; // aka threshold
    }
    public LevelGenerationValues[] levelGenerationValues;

    // The size of the perlin noise
    // Never use integer values,
    // PerlinNoise doesn't like that
    public float noiseSize = 1.01f;


    public float GetWeight()
    {
        return weight;
    }

    public float GetNoiseSize()
    {
        return noiseSize;
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
        // This is something that will be evaluated pretty often.

        return percent;
    }
}
