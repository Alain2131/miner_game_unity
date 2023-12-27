using UnityEngine;

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

    private Texture2D tex2D_single;

    private void Start()
    {
        renderTexture = new RenderTexture(resolution, resolution, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();
        computeShader.SetTexture(0, "Result", renderTexture);

        // SampleData stuff
        sData = new SampleData[1];
        int colorSize = sizeof(float) * 4;
        sampleBuffer = new ComputeBuffer(sData.Length, colorSize);
        sampleBuffer.SetData(sData);
        computeShader.SetBuffer(0, "sampleData", sampleBuffer);

        computeShader.Dispatch(0, resolution / 8, resolution / 8, 1);


        tex2D_single = new Texture2D(1, 1, TextureFormat.RGB24, false);

        tex2D_full = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
        tex2D_full.filterMode = FilterMode.Point;

        RenderTexture.active = renderTexture;
        tex2D_full.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0, false);
        RenderTexture.active = null;
        tex2D_full.Apply();

        //material.SetTexture("_MainTex", renderTexture);
        material.SetTexture("_MainTex", tex2D_full);
    }

    void Update()
    {
        Vector3 pos = sampleObject.position;
        float scale = material.GetFloat("_Scale");

        float xID = Remap(pos.x, 0f, scale, 0f, resolution);
        float yID = Remap(pos.y, 0f, -scale, 0f, resolution);

        xID = Mathf.Clamp(xID, 0, resolution-1);
        yID = Mathf.Clamp(yID, 0, resolution-1);

        idx = (int)xID;
        idy = (int)yID;

        // Only compute if necessary
        if (oldX != idx || oldY != idy)
        {
            // Sample through Texture2D.GetPixel
            RenderTexture.active = renderTexture;
            tex2D_single.ReadPixels(new Rect(idx, idy, 1, 1), 0, 0, false); // Put single pixel in Texture2D
            RenderTexture.active = null;

            //Color Cd = singlePXTex2D.GetPixel(0, 0); // Read in pixel value
            Color Cd = tex2D_full.GetPixel(idx, idy); // !! Wrong coordinates, please fix !!
            // This is clamped to 1
            Debug.Log("GetPixel : " + Cd);

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
