using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class ProcMakeLine : MonoBehaviour
{
    public int xSize = 10;
    public Object quad;
    public int lineID = -1;

    // Temporary debug variables
    public float noiseMult = 1.05f;
    public float noiseThreshold = 0.5f;

    private bool[] toGenerate;

    private GameObject[] tiles; // contains a reference to all the child tiles

    private void Start()
    {
        // Init Arrays
        toGenerate = new bool[xSize + 1];
        tiles = new GameObject[xSize];
        
        SpawnTiles();
        GenerateLine(lineID);
    }

    // Called once at level start
    private void SpawnTiles()
    {
        // Generate Tiles
        for (int x = 0; x < xSize; x++)
        {
            Vector3 pos = transform.position;

            int zRot = Random.Range(0, 5) * 90;
            Quaternion rotation = Quaternion.Euler(0, 0, zRot);

            GameObject newTile = (GameObject)GameObject.Instantiate(quad, new Vector3(pos.x + x + 0.5f, pos.y - 0.5f, pos.z), rotation, transform);
            newTile.GetComponent<tile>().ID = x;

            tiles[x] = newTile;
        }
    }

    // Called when the line is moved and reused
    public void GenerateLine(int lineID, float seed = 0f)
    {
        // We can't use lineID directly, because it doesn't update correctly inside the same frame
        // so, we pass it manually

        // temporary toGenerate bool array
        // Will have to be converted into an array of blocks to generate, with their types
        // dirt, iron, coal, rock, etc
        // If the type is null (or simply unknown), then don't generate

        for (int x = 0; x < xSize; x++)
        {
            //toGenerate[x] = Random.Range(0, 1f) > 0.35f;
            
            float noiseVal = Mathf.PerlinNoise(x * noiseMult + seed, lineID * noiseMult + seed);
            toGenerate[x] = noiseVal > noiseThreshold;

            if (this.lineID == 0) // If we're the first line
                toGenerate[x] = true;

            if (toGenerate[x])
            {
                tiles[x].GetComponent<tile>().EnableTile();
            }
            else
            {
                tiles[x].GetComponent<tile>().DisableTile();
            }
            
            // We could randomize rotation, but it's not necessary
        }
        
        // This might have to be updated on the next Update
        // It currently works because there is a setup in LevelGeneration.cs
        // that dirties the collision and updates it in FixedUpdate()
        GenerateCollision2D(toGenerate);
    }

    private void GenerateCollision2D(bool[] toGenerate)
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();

        Vector2[] corners;
        corners = new Vector2[xSize + 1];

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

                if(prev == false) // from empty to tile
                    corners[halfVertCount] = new Vector2(x + offset, 0); // top-left corner
                else // from tile to empty
                    corners[halfVertCount] = new Vector2(x + offset, -1); // bottom-right corner

                prev = !prev;
                halfVertCount++;
            }
        }
        // Create a new array, twice the vertex count
        // store tmpVerts inside of it, the second time with an offset of 1 in y
        Vector2[] verts = new Vector2[halfVertCount * 2];
        for (int i = 0, j = 0; j < halfVertCount; i+=4, j+=2)
        {
            // j   is top-left corner
            // j+1 is bottom-right corner
            verts[i+0] = new Vector2(corners[j].x,   corners[j].y);
            verts[i+1] = new Vector2(corners[j].x,   corners[j+1].y);
            verts[i+2] = new Vector2(corners[j+1].x, corners[j+1].y);
            verts[i+3] = new Vector2(corners[j+1].x, corners[j].y);
        }

        col.pathCount = halfVertCount / 2;

        Vector2[] pts = new Vector2[4];
        for (int i=0; i<col.pathCount; i++)
        {
            pts[0] = verts[i * 4 + 0];
            pts[1] = verts[i * 4 + 1];
            pts[2] = verts[i * 4 + 2];
            pts[3] = verts[i * 4 + 3];

            col.SetPath(i, pts);
        }
        
        col.transform.parent.GetComponent<CompositeCollider2D>().GenerateGeometry();
    }
    
    public void RecomputeCollision(int deletedTileID)
    {
        // Update tileDeleted array considering the newly deleted tileID
        toGenerate[deletedTileID] = false;

        GenerateCollision2D(toGenerate);
    }
}
