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
    private bool visualize_sample = false;

    // https://stackoverflow.com/questions/12413948/c-sharp-checking-if-a-variable-is-initialized
    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types
    private float? tile_world_size = null;


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
        int color_size = sizeof(float) * 4;
        sampleBuffer = new ComputeBuffer(sData.Length, color_size);
        sampleBuffer.SetData(sData);
        computeShader.SetBuffer(0, "sampleData", sampleBuffer);

        computeShader.SetBool("visualizeSample", false); // will be set to true later if needed

        computeShader.SetTexture(0, "Result", renderTexture);


        // Send world generation data stuff
        // air, dirt, coal, iron
        const int MAX_ITEMS = 50; // Matches with MAX_ITEMS in WorldGeneration.compute. They need to manually stay in sync.

        // This data will have to be automatically gathered from the Scriptable Objects.
        float[] air_percents = { 0, 0.15f };
        int[] air_depths = { 0, 5 };
        
        float[] dirt_percents = { 1 };
        int[] dirt_depths = { 0 };

        float[] coal_percents = { 0, 0.25f, 0.25f, 0.1f };
        int[] coal_depths = { 2, 4, 100, 110 };

        float[] iron_percents = { 0, 0.2f }; // 0.2f
        int[] iron_depths = { 50, 100 };

        float[] test_percents = { 0, 1, 1, 0 };
        int[] test_depths = { 5, 10, 20, 30 };


        List<int> stride_offset_list = new List<int>();
        List<float> percents_list = new List<float>();
        stride_offset_list.Add(percents_list.Count);
        percents_list.AddRange(air_percents);
        stride_offset_list.Add(percents_list.Count);
        percents_list.AddRange(dirt_percents);
        stride_offset_list.Add(percents_list.Count);
        percents_list.AddRange(coal_percents);
        stride_offset_list.Add(percents_list.Count);
        percents_list.AddRange(iron_percents);
        stride_offset_list.Add(percents_list.Count);
        
        //percents_list.AddRange(test_percents);
        //stride_offset_list.Add(percents_list.Count);
        
        if (percents_list.Count > MAX_ITEMS)
            Debug.LogError("Too many world generation items, please increase MAX_ITEMS in C# and Compute Shader. -> " + percents_list.Count + ">" + MAX_ITEMS);
        
        List<int> depths_list = new List<int>();
        depths_list.AddRange(air_depths);
        depths_list.AddRange(dirt_depths);
        depths_list.AddRange(coal_depths);
        depths_list.AddRange(iron_depths);
        //depths_list.AddRange(test_depths);
        
        if (percents_list.Count != depths_list.Count)
            Debug.LogError("percents[] and depths[] length mismatch. -> " + percents_list.Count + "!=" + depths_list.Count);

        List<float> noise_sizes_list = new List<float>()
        { 1.25f, 1.02f, 1.05f, 1.08f };

        // There is an issue where SetFloats() and SetInts() do not work properly.
        // The data needs to be arranged in sets of 4, where int and float
        // only use the first index.
        // This has no effect on the Compute Shader side.
        // https://forum.unity.com/threads/compute-shader-setfloats-broken.804585/
        // https://forum.unity.com/threads/computeshader-setints-failing-or-me-failing.669829/
        // https://cmwdexint.com/2017/12/04/computeshader-setfloats/
        List<float> PadList_Float(List<float> input_list, int max_items)
        {
            List<float> padded_list = new List<float>();
            for (int i = 0; i < max_items; i++)
            {
                if (i < input_list.Count)
                    padded_list.Add(input_list[i]);
                else
                    padded_list.Add(-1);
                padded_list.Add(0);
                padded_list.Add(0);
                padded_list.Add(0);
            }
            return padded_list;
        }

        List<int> PadList_Int(List<int> input_list, int max_items)
        {
            List<int> padded_list = new List<int>();
            for (int i = 0; i < max_items; i++)
            {
                if (i < input_list.Count)
                    padded_list.Add(input_list[i]);
                else
                    padded_list.Add(-1);
                padded_list.Add(0);
                padded_list.Add(0);
                padded_list.Add(0);
            }
            return padded_list;
        }

        List<float> padded_percents_list = PadList_Float(percents_list, MAX_ITEMS);
        List<float> padded_noise_sizes_list = PadList_Float(noise_sizes_list, MAX_ITEMS);
        List<int> padded_depths_sizes_list = PadList_Int(depths_list, MAX_ITEMS);
        List<int> padded_stride_offset_list = PadList_Int(stride_offset_list, MAX_ITEMS);

        computeShader.SetFloats("percents", padded_percents_list.ToArray());
        computeShader.SetFloats("noiseSizes", padded_noise_sizes_list.ToArray());
        computeShader.SetInts("depths", padded_depths_sizes_list.ToArray());
        computeShader.SetInts("strideOffset", padded_stride_offset_list.ToArray());

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
        /*
        CreateQuadAtPixelID(resolution * 0 + 6);
        CreateQuadAtPixelID(resolution * 1 + 6);
        CreateQuadAtPixelID(resolution * 2 + 6);

        CreateQuadAtPixelID(resolution * 3 + 5);
        CreateQuadAtPixelID(resolution * 3 + 6);
        CreateQuadAtPixelID(resolution * 3 + 7);

        CreateQuadAtPixelID(resolution * 4 + 6);
        CreateQuadAtPixelID(resolution * 5 + 6);*/
    }

    public void SetVisualize(bool mode)
    {
        visualize_sample = mode;

        computeShader.SetBool("visualizeSample", visualize_sample);
        computeShader.Dispatch(0, resolution / 8, resolution / 8, 1);
    }

    public Color SampleAtID(int pixel_ID, bool update_visualizer = true)
    {
        int idx = pixel_ID % resolution;
        int idy = pixel_ID / resolution;

        if (visualize_sample && update_visualizer)
            SetVisualizeIndex(pixel_ID);

        // A few ways to sample the data, they all give the same result
        // Don't know about performance

        // This data was the ground truth for a while, before I got the rest working properly.
        //Color Cd0 = GetColorFromCSBuffer(pixelID);

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

        return Cd;
    }

    public int PositionToPixelID(Vector3 position)
    {
        float scale = GetMaterialScale();

        int idx = Mathf.FloorToInt((position.x / scale) * resolution);
        int idy = Mathf.FloorToInt((position.y / scale) * resolution);
        idy += resolution; // there's an offset of "one page" between texture-space and world-space

        idx = Mathf.Clamp(idx, 0, resolution - 1);
        idy = Mathf.Clamp(idy, 0, resolution - 1);

        int pixel_ID = idx + (idy * resolution);
        return pixel_ID;
    }

    public Vector3 PixelIDToPosition(int pixel_ID)
    {
        int idx = pixel_ID % resolution;
        int idy = pixel_ID / resolution;
        idy -= resolution; // cancel out "one page offset"

        //float scale = GetMaterialScale();
        //float xPos = (idx * scale) / resolution;
        //float yPos = (idy * scale) / resolution;

        float tile_size = GetPixelWorldSize();
        float x_pos = idx * tile_size;
        float y_pos = idy * tile_size;

        // xPos and yPos at the bottom-left corner, so we add half the size
        x_pos += tile_size * 0.5f;
        y_pos += tile_size * 0.5f;

        return new Vector3(x_pos, y_pos, 0);
    }

    public Color SampleAtPosition(Vector3 position)
    {
        int pixel_ID = PositionToPixelID(position);

        Color Cd = SampleAtID(pixel_ID);
        return Cd;
    }

    private bool SetVisualizeIndex(int pixel_ID)
    {
        if (visualize_sample)
        {
            int idx = pixel_ID % resolution;
            int idy = pixel_ID / resolution;
            computeShader.SetInt("idx", idx);
            computeShader.SetInt("idy", idy);

            computeShader.Dispatch(0, resolution / 8, resolution / 8, 1);

            return true;
        }

        return false;
    }

    private Color GetColorFromCSBuffer(int pixel_ID)
    {
        // The problem with this method is that it's recomputing the ENTIRE Compute Shader
        // each time we need to make another sample. We don't _need_ to Visualize Sample,
        // but we'd still need to compute the shader just the same.
        // The advantage is that the data is NOT CLAMPED, which is a big deal.
        SetVisualizeIndex(pixel_ID);
        sampleBuffer.GetData(sData);

        return sData[0].color;
    }

    private float GetMaterialScale()
    {
        return material.GetFloat("_Scale");
    }

    // This could probably be optimized with better math logic
    public int GetPixelAtOffset(int pixel_ID, int offset_x, int offset_y)
    {
        int idx = pixel_ID % resolution;
        int idy = pixel_ID / resolution;

        idx += offset_x;
        idy += offset_y;

        // Sampling is Out of Bounds
        if (idx < 0 || idy < 0 || idx > resolution || idy > resolution)
            return -1;

        int final_pixel_ID = idx + (idy * resolution);
        return final_pixel_ID;
    }

    // This logic could be changed to calculate tileWorldSize on Awake()
    // Then just return that value
    // I'm leaving that in as a special sauce example,
    // no idea if this Nullable value method is bad practice
    public float GetPixelWorldSize()
    {
        if (tile_world_size.HasValue == false)
        {
            // Distance between pixelID 0 and pixelID 1
            // pixelID 0 is (0*scale)/resolution, which is 0
            // pixelID 1 is (1*scale)/resolution, which is scale/resolution
            tile_world_size = GetMaterialScale() / resolution;
        }

        return tile_world_size.Value;
    }

    // Debug Feature
    public void CreateQuadAtPixelID(int pixel_ID)
    {
        float size = GetPixelWorldSize();
        Vector3 position = PixelIDToPosition(pixel_ID);

        GameObject square = GameObject.CreatePrimitive(PrimitiveType.Quad);
        square.transform.localScale = new Vector3(size, size, size);
        square.transform.position = position;
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
