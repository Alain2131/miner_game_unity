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
    private int perlin_noise_handle;

    void Start()
    {
        gradients = new ComputeBuffer(256, sizeof(float) * 2);
        gradients.SetData(Enumerable.Range(0, 256).Select((i) => GetRandomDirection()).ToArray());

        renderTexture = new RenderTexture(resolution, resolution, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();

        perlin_noise_handle = computeShader.FindKernel("CSMain");
        computeShader.SetTexture(perlin_noise_handle, "Result", renderTexture);

        computeShader.SetFloat("res", (float)resolution);
        computeShader.SetBuffer(perlin_noise_handle, "gradients", gradients);

        material.SetTexture("_MainTex", renderTexture);
    }

    void Update()
    {
        //computeShader.SetFloat("t", (float)EditorApplication.timeSinceStartup); // doesn't work in shipped builds, will need an alternative
        computeShader.SetFloat("t", 0.0f);
        computeShader.Dispatch(perlin_noise_handle, resolution / 8, resolution / 8, 1);
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
