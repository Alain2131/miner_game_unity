using UnityEngine;

public class WorldGeneration_sample : MonoBehaviour
{
    public WorldGeneration script; // should find a better name
    public Transform sampleTransform;

    public Color result_color;
    public Vector3 result_vector;
    public Vector4 result_vector4;

    // Used to cache calculations
    // Each script that fetches the Color would
    // handle their own caching, for instance
    // oldID==newID ? oldColor : sampleColor()
    private int oldPixelID;

    private void Start()
    {
        script.SetVisualize(true);
    }

    void Update()
    {
        Vector3 position = sampleTransform.position;

        int pixelID = script.PositionToPixelID(position);
        if (pixelID != oldPixelID)
        {
            Color Cd = script.SampleAtID(pixelID);
            // Equivalent, but we've already calculated pixelID
            //Cd = script.SampleAtPosition(position);

            result_color = Cd;
            result_vector = new Vector3(Cd.r, Cd.g, Cd.b); // I don't like how Vector4 is shown, so I choose to ditch the Alpha
            result_vector4 = Cd;

            oldPixelID = pixelID;
        }
    }
}
