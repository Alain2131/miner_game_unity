using UnityEngine;

// Perhaps this could be used to write the data out if needed (would that be called "serialize" ?)
// https://docs.unity3d.com/ScriptReference/ImageConversion.html
public class Sample_from_ComputeShader : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public Texture2D tex2D_full;
    public Material material;
    public Transform sampleObject;

    private int idx;
    private int idy;
    private int oldX;
    private int oldY;

    private int resolution = 128;

    private struct SampleData
    {
        public Color color;
    }
    private ComputeBuffer sampleBuffer;
    private SampleData[] sData;

    private void Start()
    {
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

        computeShader.Dispatch(0, resolution / 8, resolution / 8, 1);


        // Same comment about the format, was using .RGB24
        // Thanks to this post for the proper format
        // https://forum.unity.com/threads/saving-fragment-position-into-textures.444652/#post-2876398
        tex2D_full = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
        tex2D_full.filterMode = FilterMode.Point;

        RenderTexture.active = renderTexture;
        tex2D_full.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0, false);
        RenderTexture.active = null;
        tex2D_full.Apply();


        // Only for visualizing, not required for sampling.
        material.SetTexture("_MainTex", renderTexture); // tex2D_full
        // "renderTexture" is required to have the proper crosshair for visualizing.
        // "tex2D_full" could work, but we'd need to copy the data each time it updates.
    }

    void Update()
    {
        Vector3 pos = sampleObject.position;
        float scale = material.GetFloat("_Scale");

        float xID = Remap(pos.x, 0f, scale, 0f, resolution);
        float yID = Remap(pos.y, 0f, scale, 0f, resolution);
        yID = Mathf.Floor(yID) + resolution;

        xID = Mathf.Clamp(xID, 0, resolution-1);
        yID = Mathf.Clamp(yID, 0, resolution-1);

        idx = (int)xID;
        idy = (int)yID;

        // Only compute if necessary
        if (oldX != idx || oldY != idy)
        {
            bool visualizeSample = true;
            if(visualizeSample)
            {
                // The problem with this method is that it's recomputing the ENTIRE Compute Shader
                // each time we need to make another sample. We don't _need_ to Visualize Sample,
                // but we'd still need to compute the shader just the same.
                // The advantage is that the data is NOT CLAMPED, which is a big deal.
                computeShader.SetInt("idx", idx);
                computeShader.SetInt("idy", idy);
                computeShader.SetBool("visualizeSample", true);

                computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
                sampleBuffer.GetData(sData);
                //Debug.Log("CS result\t\t: " + sData[0].color);
            }


            // A few ways to sample the data, they all give the same result
            // Don't know about performance

            // https://docs.unity3d.com/ScriptReference/Texture2D.GetPixel.html
            Color Cd = tex2D_full.GetPixel(idx, idy);

            // https://docs.unity3d.com/ScriptReference/Texture2D.GetPixels.html
            Color[] Cxyz = tex2D_full.GetPixels();
            Color Cd1 = Cxyz[idx + (idy * resolution)];

            // https://docs.unity3d.com/ScriptReference/Texture2D.GetPixelData.html
            var data = tex2D_full.GetPixelData<Color>(0); // I can pass in Vector4 as a type, and it works fine too
            Color Cd2 = data[idx + (idy * resolution)];

            // A 'lil hack
            if (!visualizeSample)
                sData[0].color = Cd;



            bool printDebug = false;
            bool differentColors = sData[0].color != Cd || sData[0].color != Cd1 || sData[0].color != Cd2;
            if (printDebug || differentColors)
            {
                Debug.Log("CS result\t\t: " + sData[0].color);
                Debug.Log("GetPixel (full)\t\t: " + Cd);
                Debug.Log("GetPixel (new1)\t: " + Cd1);
                Debug.Log("GetPixel (new2)\t: " + Cd2);
                Debug.Log("------------------------------------------");
            }
            

            oldX = idx;
            oldY = idy;
        }
    }

    private float Remap(float value, float oMin, float oMax, float nMin, float nMax)
    {
        float fromAbs = value - oMin;
        float fromMaxAbs = oMax - oMin;

        float normal = fromAbs / fromMaxAbs;

        float toMaxAbs = nMax - nMin;
        float toAbs = toMaxAbs * normal;

        float to = toAbs + oMin;
        return to;
    }
    
    private void OnApplicationQuit()
    {
        sampleBuffer.Dispose();
    }

    /*
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
    */
}
