using System.Collections.Generic;
using UnityEngine;

// Perhaps this could be used to write the data out if needed (would that be called "serialize" ?)
// https://docs.unity3d.com/ScriptReference/ImageConversion.html
public class WorldGeneration : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture; // no need to be public, it's just fun to see in the Inspector
    public Texture2D tex2D; // no need to be public, it's just fun to see in the Inspector

    private Material material;

    private int resolution = 128;
    private bool visualizeSample = false;

    private struct SampleData
    {
        public Color color;
    }

    private ComputeBuffer sampleBuffer;
    private SampleData[] sData;

    // Next step is to write out the data
    // we also need the dug up tiles texture
    // Dug up tiles might need a "expand size" to grow height by X
    // instead of growing by 1 each time we reach a new depth.
    // Or maybe we can just make a 1000-sized texture ? (or whatever, could be 10K)
    // Do we want to split in different textures over that (arbitrary) size ?
    
    // Also want to align it all properly in the world,
    // so that one pixel takes a full unit. Make sure sampling still works.

    // Collision as well

    // Make tiles shader, consuming this script's tex2D
    // We'll want rounded corners and whatnot
    // Will use an Atlas with each tiles' texture in the order of the tileIDs
    // Dig animation will have to be in there as well. A 0-1 parameter, 1 is fully dug
    // At 1, the dug up tiles texture is updated, so it removes the tile "properly",
    // after which we can set the parameter back to 0.


    private void Awake()
    {
        //material = GetComponent<Material>(); // Always return Null, no idea why.
        material = GetComponent<MeshRenderer>().material;

        // Format is important, I did not use a format before and it was clamping the heck out of the data (regardless of depth)
        renderTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
        renderTexture.enableRandomWrite = true;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();

        // SampleData stuff
        sData = new SampleData[1];
        int colorSize = sizeof(float) * 4;
        sampleBuffer = new ComputeBuffer(sData.Length, colorSize);
        sampleBuffer.SetData(sData);
        computeShader.SetBuffer(0, "sampleData", sampleBuffer);

        computeShader.SetBool("visualizeSample", false); // will be set to true later if needed

        computeShader.SetTexture(0, "Result", renderTexture);


        // Send world generation data stuff
        // air, dirt, coal, iron
        const int maxItems = 50; // matches with maxItems in Compute Shader. They need to stay in sync, manually

        // This data will have to be automatically gathered from the Scriptable Objects.
        float[] airPercents = { 0, 0.15f };
        int[] airDepths = { 0, 5 };
        
        float[] dirtPercents = { 1 };
        int[] dirtDepths = { 0 };

        float[] coalPercents = { 0, 0.25f, 0.25f, 0.1f };
        int[] coalDepths = { 2, 4, 100, 110 };

        float[] ironPercents = { 0, 0.2f }; // 0.2f
        int[] ironDepths = { 50, 100 };

        float[] testPercents = { 0, 1, 1, 0 };
        int[] testDepths = { 5, 10, 20, 30 };


        List<int> strideOffsetList = new List<int>();
        List<float> percentsList = new List<float>();
        strideOffsetList.Add(percentsList.Count);
        percentsList.AddRange(airPercents);
        strideOffsetList.Add(percentsList.Count);
        percentsList.AddRange(dirtPercents);
        strideOffsetList.Add(percentsList.Count);
        percentsList.AddRange(coalPercents);
        strideOffsetList.Add(percentsList.Count);
        percentsList.AddRange(ironPercents);
        strideOffsetList.Add(percentsList.Count);
        
        //percentsList.AddRange(testPercents);
        //strideOffsetList.Add(percentsList.Count);
        
        if (percentsList.Count > maxItems)
            Debug.LogError("Too many world generation items, please increase maxItems in C# and Compute Shader. -> " + percentsList.Count + ">" + maxItems);
        
        List<int> depthsList = new List<int>();
        depthsList.AddRange(airDepths);
        depthsList.AddRange(dirtDepths);
        depthsList.AddRange(coalDepths);
        depthsList.AddRange(ironDepths);
        //depthsList.AddRange(testDepths);
        
        if (percentsList.Count != depthsList.Count)
            Debug.LogError("percents[] and depths[] length mismatch. -> " + percentsList.Count + "!=" + depthsList.Count);

        List<float> noiseSizesList = new List<float>()
        { 1.25f, 1.02f, 1.05f, 1.08f };

        // There is an issue where SetFloats() and SetInts() do not work properly.
        // The data needs to be arranged in sets of 4, where int and float
        // only use the first index.
        // This has no effect on the Compute Shader side.
        // https://forum.unity.com/threads/compute-shader-setfloats-broken.804585/
        // https://forum.unity.com/threads/computeshader-setints-failing-or-me-failing.669829/
        // https://cmwdexint.com/2017/12/04/computeshader-setfloats/
        List<float> PadList_Float(List<float> inputList, int maxItems)
        {
            List<float> paddedList = new List<float>();
            for (int i = 0; i < maxItems; i++)
            {
                if (i < inputList.Count)
                    paddedList.Add(inputList[i]);
                else
                    paddedList.Add(-1);
                paddedList.Add(0);
                paddedList.Add(0);
                paddedList.Add(0);
            }
            return paddedList;
        }

        List<int> PadList_Int(List<int> inputList, int maxItems)
        {
            List<int> paddedList = new List<int>();
            for (int i = 0; i < maxItems; i++)
            {
                if (i < inputList.Count)
                    paddedList.Add(inputList[i]);
                else
                    paddedList.Add(-1);
                paddedList.Add(0);
                paddedList.Add(0);
                paddedList.Add(0);
            }
            return paddedList;
        }

        List<float> paddedPercentsList = PadList_Float(percentsList, maxItems);
        List<float> paddedNoiseSizesList = PadList_Float(noiseSizesList, maxItems);
        List<int> paddedDepthsSizesList = PadList_Int(depthsList, maxItems);
        List<int> paddedStrideOffsetList = PadList_Int(strideOffsetList, maxItems);

        computeShader.SetFloats("percents", paddedPercentsList.ToArray());
        computeShader.SetFloats("noiseSizes", paddedNoiseSizesList.ToArray());
        computeShader.SetInts("depths", paddedDepthsSizesList.ToArray());
        computeShader.SetInts("strideOffset", paddedStrideOffsetList.ToArray());

        computeShader.Dispatch(0, resolution / 8, resolution / 8, 1);


        // Same comment about the format, was using .RGB24
        // Thanks to this post for the proper format
        // https://forum.unity.com/threads/saving-fragment-position-into-textures.444652/#post-2876398
        tex2D = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
        tex2D.filterMode = FilterMode.Point;

        RenderTexture.active = renderTexture;
        tex2D.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0, false);
        RenderTexture.active = null;
        tex2D.Apply();


        // Only for visualizing, not required for sampling.
        material.SetTexture("_MainTex", renderTexture); // tex2D_full
        // "renderTexture" is required to have the proper crosshair for visualizing.
        // "tex2D_full" could work, but we'd need to copy the data each time it updates.
    }

    public void SetVisualize(bool mode)
    {
        visualizeSample = mode;

        computeShader.SetBool("visualizeSample", visualizeSample);
        computeShader.Dispatch(0, resolution / 8, resolution / 8, 1);
    }

    public Color SampleAtID(int pixelID)
    {
        int idx = pixelID % resolution;
        int idy = pixelID / resolution;

        if (visualizeSample)
            SetVisualizeIndex(pixelID);

        // A few ways to sample the data, they all give the same result
        // Don't know about performance

        // This data was the ground truth for a while, before I got the rest working properly.
        Color Cd0 = GetColorFromCSBuffer(pixelID);

        // https://docs.unity3d.com/ScriptReference/Texture2D.GetPixel.html
        Color Cd = tex2D.GetPixel(idx, idy);

        // https://docs.unity3d.com/ScriptReference/Texture2D.GetPixels.html
        //Color[] Cxyz = tex2D.GetPixels();
        //Color Cd1 = Cxyz[pixelID];

        // https://docs.unity3d.com/ScriptReference/Texture2D.GetPixelData.html
        //var data = tex2D.GetPixelData<Color>(0); // I can pass in Vector4 as a type, and it works fine too
        //Color Cd2 = data[pixelID];

        /*
        bool printDebug = false;
        bool differentColors = Cd != Cd1 || Cd != Cd2 || Cd != Cd0;
        if (printDebug || differentColors)
        {
            Debug.Log("CS result\t\t: " + Cd0);
            Debug.Log("GetPixel (full)\t\t: " + Cd);
            Debug.Log("GetPixel (new1)\t: " + Cd1);
            Debug.Log("GetPixel (new2)\t: " + Cd2);
            Debug.Log("------------------------------------------");
        }
        //*/

        return Cd0;
    }

    public int PositionToPixelID(Vector3 position)
    {
        float scale = GetMaterialScale();

        int idx = Mathf.FloorToInt((position.x / scale) * resolution);
        int idy = Mathf.FloorToInt((position.y / scale) * resolution);
        idy += resolution; // there's an offset of "one page" between texture-space and world-space

        idx = Mathf.Clamp(idx, 0, resolution - 1);
        idy = Mathf.Clamp(idy, 0, resolution - 1);

        int pixelID = idx + (idy * resolution);
        return pixelID;
    }

    public Color SampleAtPosition(Vector3 position)
    {
        int pixelID = PositionToPixelID(position);

        Color Cd = SampleAtID(pixelID);
        return Cd;
    }

    private bool SetVisualizeIndex(int pixelID)
    {
        if (visualizeSample)
        {
            int idx = pixelID % resolution;
            int idy = pixelID / resolution;
            computeShader.SetInt("idx", idx);
            computeShader.SetInt("idy", idy);

            computeShader.Dispatch(0, resolution / 8, resolution / 8, 1);

            return true;
        }

        return false;
    }

    private Color GetColorFromCSBuffer(int pixelID)
    {
        // The problem with this method is that it's recomputing the ENTIRE Compute Shader
        // each time we need to make another sample. We don't _need_ to Visualize Sample,
        // but we'd still need to compute the shader just the same.
        // The advantage is that the data is NOT CLAMPED, which is a big deal.
        SetVisualizeIndex(pixelID);
        sampleBuffer.GetData(sData);

        return sData[0].color;
    }

    private float GetMaterialScale()
    {
        return material.GetFloat("_Scale");
    }


    private void OnApplicationQuit()
    {
        sampleBuffer.Dispose();
    }

    /*
    // Honestly, I don't really remember what that's for. I'm leaving it in in case it might be useful later.
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(254, 254, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }

        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

        Graphics.Blit(renderTexture, dest);
    }
    //*/
}
