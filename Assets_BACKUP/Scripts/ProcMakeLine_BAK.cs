using UnityEngine;

// MeshRenderer only present for debug visualisation of the generated collision
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ProcMakeLine_BAK : MonoBehaviour
{
    public int xSize = 10;

    public Object quad;

    private bool[] toGenerate;

    private void Awake()
    {
        // temporary toGenerate bool array
        // Will have to be converted into an array of blocks to generate, with their types
        // dirt, iron, coal, rock, etc
        // If the type is null (or simply unknown), then don't generate
        toGenerate = new bool[xSize + 1]; // +1 to fix a list out of range
        for (int x = 0; x < xSize; x++)
        {
            toGenerate[x] = Random.Range(0, 1f) > 0.035f;
        }

        GenerateTiles(toGenerate);
        GenerateCollision(toGenerate);
    }

    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;

    private void GenerateTiles(bool[] toGenerate)
    {
        // Generate Tiles
        for (int x = 0; x < xSize; x++)
        {
            if (toGenerate[x])
            {
                int zRot = Random.Range(0, 5) * 90;
                Vector3 pos = transform.position;
                Quaternion rotation = Quaternion.Euler(0, 0, zRot);

                GameObject newTile = (GameObject)GameObject.Instantiate(quad, new Vector3(x + pos.x + 0.5f, pos.y + 0.5f, pos.z), rotation, transform);
                newTile.GetComponent<tile>().ID = x;
            }
        }
    }

    private void GenerateCollision(bool[] toGenerate)
    {
        // Generate Custom Collision
        mesh = new Mesh();
        mesh.name = "Custom Collision";

        // tmpVerts contains only one line of vertices, not enough to generate quads
        Vector3[] tmpVerts;
        tmpVerts = new Vector3[xSize + 1];

        bool prev = false;
        int halfVertCount = 0;
        for (int x = 0; x <= xSize; x++)
        {
            bool same = !(prev ^ toGenerate[x]);
            if (!same) // state switched
            {
                // Make collision a bit smaller compared to the actual block
                float offset = 0.05f;
                if (prev) // Invert offset if previous is a block (and current isn't)
                    offset *= -1;

                tmpVerts[halfVertCount] = new Vector3(x + offset, 0, 0);
                prev = !prev;
                halfVertCount++;
            }
        }
        // Create a new array, twice the vertex count
        // store tmpVerts inside of it, the second time with an offset of 1 in y
        vertices = new Vector3[halfVertCount * 2];
        for (int y = 0, i = 0; y < 2; y++)
        {
            for (int x = 0; x < halfVertCount; x++, i++)
            {
                vertices[i] = new Vector3(tmpVerts[x].x, y, 0);
            }
        }
        mesh.vertices = vertices;

        // half the halfVertCount (there is two verts for one quad)
        // then multiply by six, for there is three verts in one tri, and two tris in a quad
        int triangleCount = (int)(halfVertCount * 0.5f) * 6; // halfVertCount is always an even number
        triangles = new int[triangleCount];

        for (int x = 0, i = 0; x < halfVertCount; x += 2, i++)
        {
            int xoffset = i * 6;
            triangles[xoffset] = x;
            triangles[xoffset + 1] = x + halfVertCount;
            triangles[xoffset + 2] = x + 1;

            triangles[xoffset + 3] = x + 1;
            triangles[xoffset + 4] = x + halfVertCount;
            triangles[xoffset + 5] = x + 1 + halfVertCount;
        }
        mesh.triangles = triangles;

        // For visualisation only
        // GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().enabled = false;

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    
    public void RecomputeCollision(int deletedTileID)
    {
        // Update tileDeleted array considering the newly deleted tileID
        toGenerate[deletedTileID] = false;

        GenerateCollision(toGenerate);
    }
}
