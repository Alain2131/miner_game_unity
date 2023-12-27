using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ComputeShader_Noise : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public Material material;
    
    private int resolution = 128;

    private ComputeBuffer gradients;
    private int perlinNoiseHandle;

    void Start()
    {
        gradients = new ComputeBuffer(256, sizeof(float) * 2);
        gradients.SetData(Enumerable.Range(0, 256).Select((i) => GetRandomDirection()).ToArray());

        renderTexture = new RenderTexture(resolution, resolution, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();

        perlinNoiseHandle = computeShader.FindKernel("CSMain");
        computeShader.SetTexture(perlinNoiseHandle, "Result", renderTexture);

        computeShader.SetFloat("res", (float)resolution);
        computeShader.SetBuffer(perlinNoiseHandle, "gradients", gradients);

        material.SetTexture("_MainTex", renderTexture);
    }

    void Update()
    {
        computeShader.SetFloat("t", (float)EditorApplication.timeSinceStartup);
        computeShader.Dispatch(perlinNoiseHandle, resolution / 8, resolution / 8, 1);
    }

    private void OnApplicationQuit()
    {
        gradients.Dispose();
    }

    private static Vector2 GetRandomDirection()
    {
        return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
    }
}
