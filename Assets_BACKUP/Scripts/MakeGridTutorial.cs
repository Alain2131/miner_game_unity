// Tutorial : https://catlikecoding.com/unity/tutorials/procedural-grid/

using UnityEngine;
//using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MakeGridTutorial : MonoBehaviour
{
    public int xSize = 1, ySize = 1;

    private void Awake()
    {
        //StartCoroutine(Generate());
        Generate();
    }

    private Vector3[] vertices;
    private Mesh mesh;

    //private IEnumerator Generate()
    private void Generate()
    {
        //WaitForSeconds wait = new WaitForSeconds(0.1f);

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for(int i=0, y=0; y<=ySize; y++)
        {
            for(int x=0; x<=xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xSize * ySize * 6];

        // My Loop
        for (int y = 0; y < ySize; y++)
        {
            int yoffset = y * (xSize * 6);
            int yoffset1 = ((xSize + 1) * y);
            for (int x = 0; x < xSize; x++)
            {
                int xoffset = x * 6;
                triangles[xoffset + yoffset]      = x +             yoffset1;
                triangles[xoffset + yoffset + 1 ] = x + 1 + xSize + yoffset1;
                triangles[xoffset + yoffset + 2 ] = x + 1 +         yoffset1;

                triangles[xoffset + yoffset + 3 ] = x + 1 +         yoffset1;
                triangles[xoffset + yoffset + 4 ] = x + 1 + xSize + yoffset1;
                triangles[xoffset + yoffset + 5 ] = x + 2 + xSize + yoffset1;
            }
        }

        /*
        // Tutorial's Loop
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        //*/

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Will generate an error of there is no MeshCollider Component
        GetComponent<MeshCollider>().sharedMesh = mesh;
        //yield return wait;
    }

    private void OnDrawGizmos()
    {
        if(vertices == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for(int i=0; i<vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
