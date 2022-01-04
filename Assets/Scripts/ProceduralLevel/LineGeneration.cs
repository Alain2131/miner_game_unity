using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class LineGeneration : MonoBehaviour
{
    public int          xSize = 10;
    public Object       tileObject;
    public int          lineID = -1; // Basically, depth. We don't need to see it in Unity, but it's helpful for debugging. It could be set to readonly
    public AllTilesInfo allTilesInfo;

    private bool[]       airTiles;     // True is a tile, False is air/digged up
    private TileScript[] lineTiles;    // References all child tiles in the line. Only modified on Start.
    private TileInfo[]   tilesInfo; // Init'ed in Start from allTilesObject

    private void Start()
    {
        // Init Arrays
        airTiles = new bool[xSize + 1];
        lineTiles = new TileScript[xSize];
        tilesInfo = allTilesInfo.GetAllTiles();

        SpawnTiles();
    }

    private void SpawnTiles() // Called once, at level start
    {
        // Generate all Tiles (digged-up tiles and air will be disabled later)
        for (int x = 0; x < xSize; x++)
        {
            Vector3 pos = transform.position;
            pos.x += x + 0.5f;
            pos.y -= 0.5f;

            int zRot = Random.Range(0, 5) * 90;
            Quaternion rotation = Quaternion.Euler(0, 0, zRot);

            GameObject newTile = (GameObject)GameObject.Instantiate(tileObject, pos, rotation, transform);
            newTile.GetComponent<TileScript>().x_ID = x;

            lineTiles[x] = newTile.GetComponent<TileScript>();
        }

        GenerateLine(lineID);
    }

    // Called on Start and when the line is moved/reused
    public void GenerateLine(int lineID, float seed = 0f)
    {
        // We can't use lineID directly,
        // because it doesn't update correctly inside the same frame
        // So, we pass it manually

        float noiseVal, threshold, noiseSize;
        float bias = 0.001f; // smol bias to remove 0 in noiseVal
        for (int x = 0; x < xSize; x++)
        {
            TileScript currentTile = lineTiles[x];

            // Update uniqueID
            int uniqueID = lineID * xSize + x;
            currentTile.uniqueID = uniqueID;

            if (isTileDugUp(uniqueID) == true) // Handle the tiles already dug up
                airTiles[x] = false;
            else
            {
                // Add Air
                noiseSize = allTilesInfo.GetAir().noiseSize;
                threshold = allTilesInfo.GetAir().GetSpawnPercent(lineID);

                noiseVal = Mathf.PerlinNoise(x * noiseSize + seed, lineID * noiseSize + seed);
                noiseVal = Mathf.Clamp01(noiseVal) + bias;
                airTiles[x] = noiseVal > threshold;

                if (this.lineID == 0) // If we're the first line,
                    airTiles[x] = true; // remove all air.
            }
            
            // Disable air tile, Enable non-air tile
            currentTile.SetEnabled(airTiles[x]);


            if (airTiles[x]) // If we know it's not an air tile
            {
                // Loop through all the TileInfo, and place the first "valid" tile
                foreach(TileInfo tileInfo in tilesInfo)
                {
                    noiseSize = tileInfo.GetNoiseSize();
                    threshold = tileInfo.GetSpawnPercent(lineID);

                    if (threshold == 0)
                        continue;
                    
                    noiseVal = Mathf.PerlinNoise(x * noiseSize + seed, lineID * noiseSize + seed);
                    noiseVal = Mathf.Clamp01(noiseVal) + bias;
                    if (noiseVal < threshold)
                    {
                        currentTile.SetMaterial(tileInfo.GetMaterial());
                        currentTile.tileInfo = tileInfo;
                        break;
                    }
                }
            }

            // We could randomize rotation,
            // but it's not necessary when re-using the line.
            // It is already randomised on spawn.
        }
        
        // This might have to be updated on the next Update
        // It currently works because there is a setup in LevelGeneration.cs
        // that dirties the collision and updates it in FixedUpdate()
        GenerateCollision2D(airTiles); // this is quite expensive, but necessary
    }

    private bool isTileDugUp(int uniqueID)
    {
        // I wonder how fast that will be when the List will be thousands long
        bool isDug = GameManager.Instance.tilesDugUp.Contains(uniqueID);
        return isDug;
    }

    private void GenerateCollision2D(bool[] toGenerate)
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();

        Vector2[] corners = new Vector2[xSize + 1];

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
                {
                    // top-left corner
                    corners[halfVertCount].x = x + offset;
                    corners[halfVertCount].y = 0;
                }
                else // from tile to empty
                {
                    // bottom-right corner
                    corners[halfVertCount].x = x + offset;
                    corners[halfVertCount].y = -1;
                }

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
            verts[i + 0].x = corners[j].x;
            verts[i + 0].y = corners[j].y;

            verts[i + 1].x = corners[j].x;
            verts[i + 1].y = corners[j+1].y;

            verts[i + 2].x = corners[j+1].x;
            verts[i + 2].y = corners[j+1].y;

            verts[i + 3].x = corners[j+1].x;
            verts[i + 3].y = corners[j].y;
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
        airTiles[deletedTileID] = false;

        GenerateCollision2D(airTiles);
    }
}
