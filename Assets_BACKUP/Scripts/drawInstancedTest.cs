using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class drawInstancedTest : MonoBehaviour
{
    private Mesh quad;

    void Awake()
    {
        MakeQuad();

        int xCount = 100;
        int yCount = 500;
        CombineInstance[] combine = new CombineInstance[xCount * yCount];

        Vector3 scale = new Vector3(1, 1, 1);

        for (int y = 0, i = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++, i++)
            {
                int zRot = Random.Range(0, 5) * 90;
                Quaternion rotation = Quaternion.Euler(0, 0, zRot);

                combine[i].mesh = quad;
                combine[i].transform = Matrix4x4.TRS(new Vector3(x, -y, 0), rotation, scale);
            }
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "CombinedMesh";

        if ((xCount * yCount)*4 > 65535)
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        
        combinedMesh.CombineMeshes(combine);
        GetComponent<MeshFilter>().mesh = combinedMesh;
    }

    private void MakeQuad()
    {
        quad = new Mesh();
        quad.Clear();
        quad.name = "Quad";

        Vector3[] vertices = new Vector3[4];
        float a = 0.5f;
        vertices[0] = new Vector3(-a, -a, 0);
        vertices[1] = new Vector3(a, -a, 0);
        vertices[2] = new Vector3(-a, a, 0);
        vertices[3] = new Vector3(a, a, 0);

        Vector2[] uv = new Vector2[4];
        Vector4[] tangents = new Vector4[4];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= 1; y++)
        {
            for (int x = 0; x <= 1; x++, i++)
            {
                uv[i] = new Vector2((float)x, (float)y);
                tangents[i] = tangent;
            }
        }

        quad.vertices = vertices;
        quad.uv = uv;
        quad.tangents = tangents;

        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;

        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 3;

        quad.triangles = triangles;
        quad.RecalculateNormals();
    }
}
