using UnityEngine;

public class Distance_to_Object : MonoBehaviour
{
    public Transform thing;
    public Material Material;

    void Update()
    {
        Vector4 pos = thing.position;
        float s = thing.localScale.x * 0.5f;

        Material.SetVector("_Origin", pos);
        Material.SetFloat("_Maximum", s);
    }
}
