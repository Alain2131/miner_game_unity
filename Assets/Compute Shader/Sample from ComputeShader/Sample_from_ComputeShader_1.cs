using UnityEngine;

public class Sample_from_ComputeShader_1 : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public Material material;
    public Transform sampleObject;

    private int idx = 0;
    private int idy = 0;
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
        sData = new SampleData[1];
        int colorSize = sizeof(float) * 4;
        sampleBuffer = new ComputeBuffer(sData.Length, colorSize);
        sampleBuffer.SetData(sData);

        computeShader.SetBuffer(0, "sampleData", sampleBuffer);

        renderTexture = new RenderTexture(resolution, resolution, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();
        computeShader.SetTexture(0, "Result", renderTexture);

        material.SetTexture("_MainTex", renderTexture);
    }

    void Update()
    {
        Vector3 pos = sampleObject.position;
        float scale = material.GetFloat("_Scale");

        float xID = Remap(pos.x, 0f, scale, 0f, resolution);
        float yID = Remap(pos.y, 0f, scale, 0f, resolution);
        yID = Mathf.Floor(yID) + resolution;

        xID = Mathf.Clamp(xID, 0, resolution - 1);
        yID = Mathf.Clamp(yID, 0, resolution - 1);

        idx = (int)xID;
        idy = (int)yID;

        // Only compute if necessary
        if (oldX != idx || oldY != idy)
        {
            computeShader.SetFloat("idx", idx);
            computeShader.SetFloat("idy", idy);
            computeShader.SetBool("visualizeSample", true);

            computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
            sampleBuffer.GetData(sData);
            //sampleBuffer.Dispose();

            // Sample through Compute Buffer
            // Gets the full value, even higher than 1
            Debug.Log("CS result : " + sData[0].color);

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
    /*
    private void OnApplicationQuit()
    {
        sampleBuffer.Dispose();
    }*/

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
