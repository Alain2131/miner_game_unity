using UnityEngine;

public class SampleExample : MonoBehaviour
{
    public Sample_from_ComputeShader script; // should find a better name
    public Transform sampleTransform;

    public Color result_color;
    public Vector3 result_vector;
    

    private void Start()
    {
        script.SetVisualize(true);
    }

    void Update()
    {
        Vector3 pos = sampleTransform.position;
        Color Cd = script.SampleAtPosition(pos);
        result_color = Cd;
        result_vector = new Vector3(Cd.r, Cd.g, Cd.b); // I don't like how Vector4 is shown, so I choose to ditch the Alpha
    }
}
