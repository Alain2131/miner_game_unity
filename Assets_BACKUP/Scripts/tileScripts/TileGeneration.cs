using UnityEngine;
using System.Collections.Generic; // Includes List

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TileGeneration : MonoBehaviour
{
    private Mesh quad;

    private Vector3[] vertices;
    private Mesh mesh;

    internal int tileType;

    private void Awake()
    {
        MakeQuad();
    }

    public void GenerateTiles(List<int[]> allLines, List<bool[]> dugTiles)
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.Clear();
        mesh.name = transform.gameObject.name + "_mesh";

        int xSize = allLines[0].Length;
        int ySize = allLines.Count;

        CombineInstance instance = new CombineInstance();
        List<CombineInstance> combine = new List<CombineInstance>();

        Vector3 scale = new Vector3(1, 1, 1);

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                if(allLines[y][x] == tileType && dugTiles[y][x] == false)
                {
                    int zRot = Random.Range(0, 5) * 90;
                    Quaternion rotation = Quaternion.Euler(0, 0, zRot);

                    instance.mesh = quad;
                    instance.transform = Matrix4x4.TRS(new Vector3(x, -y, 0), rotation, scale);

                    combine.Add(instance);
                }
            }
        }

        if ((xSize * ySize) * 4 > 65535)
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        // Convert combine List to an Array
        CombineInstance[] combineArray = new CombineInstance[combine.Count];
        for(int i=0; i<combine.Count; i++)
        {
            combineArray[i] = combine[i];
        }
        mesh.CombineMeshes(combineArray);
        //GetComponent<MeshFilter>().mesh = mesh;



        /*
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y, 0);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;





        int[] line = new int[xSize];
        for (int y=0; y<ySize; y++)
        {
            line = allLines[y];
            for (int x=0; x<xSize; x++)
            {

            }
        }
        */
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
