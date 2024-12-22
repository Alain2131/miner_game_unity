using UnityEngine;

// Perhaps this could be used to write the data out if needed (would that be called "serialize" ?)
// https://docs.unity3d.com/ScriptReference/ImageConversion.html
public class Sample_from_ComputeShader : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture; // no need to be public, it's just fun to see in the Inspector
    public Texture2D tex2D; // no need to be public, it's just fun to see in the Inspector

    private Material material;

    private int resolution = 128;
    private bool visualize_sample = false;

    private struct SampleData
    {
        public Color color;
    }
    private ComputeBuffer sample_buffer;
    private SampleData[] sData;


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
        sample_buffer = new ComputeBuffer(sData.Length, color_size);
        sample_buffer.SetData(sData);
        computeShader.SetBuffer(0, "sampleData", sample_buffer);
        computeShader.SetBool("visualizeSample", false); // will be set to true later if needed

        computeShader.SetTexture(0, "Result", renderTexture);

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
        visualize_sample = mode;

        computeShader.SetBool("visualizeSample", visualize_sample);
        computeShader.Dispatch(0, resolution / 8, resolution / 8, 1);
    }

    public Color SampleAtID(int pixel_ID)
    {
        int idx = pixel_ID % resolution;
        int idy = pixel_ID / resolution;

        if (visualize_sample)
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
        idy += resolution; // there's an offset of "one page" between texture-space and world-space (so it seems)

        idx = Mathf.Clamp(idx, 0, resolution - 1);
        idy = Mathf.Clamp(idy, 0, resolution - 1);

        int pixel_ID = idx + (idy * resolution);
        return pixel_ID;
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
        sample_buffer.GetData(sData);

        return sData[0].color;
    }

    private float GetMaterialScale()
    {
        return material.GetFloat("_Scale");
    }


    private void OnApplicationQuit()
    {
        sample_buffer.Dispose();
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
