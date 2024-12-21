using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class LineGeneration : MonoBehaviour
{
    private GameManager gameManager;
    public AllTilesInfo allTilesInfo;
    private int xSize;

    public Object tileObject;
    public int lineID = -1; // Basically, depth. We don't need to see it in Unity, but it's helpful for debugging. It could be set to readonly
    
    private bool[] airTiles; // True is a tile, False is air/digged up
    private TileScript[] lineTiles; // References all child tiles in the line. Only modified on Start.
    private TileInfo[] tilesInfo; // Init'ed in Start from allTilesObject

    private void Start()
    {
        gameManager = GameManager.Instance;
        xSize = gameManager.LevelXSize;

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

            int z_rot = Random.Range(0, 5) * 90;
            Quaternion rotation = Quaternion.Euler(0, 0, z_rot);

            GameObject new_tile = (GameObject)GameObject.Instantiate(tileObject, pos, rotation, transform);
            new_tile.GetComponent<TileScript>().x_ID = x;

            lineTiles[x] = new_tile.GetComponent<TileScript>();
        }

        GenerateLine(lineID);
    }

    // Called on Start and when the line is moved/reused
    public void GenerateLine(int line_ID, float seed = 0f)
    {
        // We can't use lineID directly,
        // because it doesn't update correctly inside the same frame
        // So, we pass it manually

        float noise_value, threshold, noise_size;
        float bias = 0.001f; // smol bias to remove 0 in noiseVal
        for (int x = 0; x < xSize; x++)
        {
            TileScript current_tile = lineTiles[x];

            // Update uniqueID
            int unique_ID = line_ID * xSize + x;
            current_tile.uniqueID = unique_ID;

            if (IsTileDugUp(unique_ID) == true) // Handle the tiles already dug up
                airTiles[x] = false;
            else
            {
                // Add Air
                noise_size = allTilesInfo.airTile.noiseSize;
                threshold = allTilesInfo.airTile.GetSpawnPercent(line_ID);

                noise_value = Mathf.PerlinNoise(x * noise_size + seed, line_ID * noise_size + seed);
                noise_value = Mathf.Clamp01(noise_value) + bias;
                airTiles[x] = noise_value > threshold;

                if (this.lineID == 0) // If we're the first line,
                    airTiles[x] = true; // remove all air.
            }
            
            // Disable air tile, Enable non-air tile
            current_tile.SetEnabled(airTiles[x]);


            if (airTiles[x]) // If we know it's not an air tile
            {
                // Loop through all the TileInfo, and place the first "valid" tile
                foreach(TileInfo tile_info in tilesInfo)
                {
                    noise_size = tile_info.GetNoiseSize();
                    threshold = tile_info.GetSpawnPercent(line_ID);

                    if (threshold == 0)
                        continue;
                    
                    noise_value = Mathf.PerlinNoise(x * noise_size + seed, line_ID * noise_size + seed);
                    noise_value = Mathf.Clamp01(noise_value) + bias;
                    if (noise_value < threshold)
                    {
                        current_tile.SetMaterial(tile_info.GetMaterial());
                        current_tile.tileInfo = tile_info;
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

    private bool IsTileDugUp(int unique_ID)
    {
        // I wonder how fast that will be when the List will be thousands long
        bool is_dug = GameManager.Instance.tilesDugUp.Contains(unique_ID);
        return is_dug;
    }

    private void GenerateCollision2D(bool[] to_generate)
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();

        Vector2[] corners = new Vector2[xSize + 1];

        bool prev = false;
        int half_vert_count = 0;
        for (int x = 0; x <= xSize; x++)
        {
            bool same = !(prev ^ to_generate[x]);
            if (!same) // state switched
            {
                // Make collision a bit smaller compared to the actual block
                float offset = 0.05f;
                if (prev) // Invert offset if previous is a block (and current isn't)
                    offset *= -1;

                if(prev == false) // from empty to tile
                {
                    // top-left corner
                    corners[half_vert_count].x = x + offset;
                    corners[half_vert_count].y = 0;
                }
                else // from tile to empty
                {
                    // bottom-right corner
                    corners[half_vert_count].x = x + offset;
                    corners[half_vert_count].y = -1;
                }

                prev = !prev;
                half_vert_count++;
            }
        }
        // Create a new array, twice the vertex count
        // store tmpVerts inside of it, the second time with an offset of 1 in y
        Vector2[] verts = new Vector2[half_vert_count * 2];
        for (int i = 0, j = 0; j < half_vert_count; i+=4, j+=2)
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

        col.pathCount = half_vert_count / 2;

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
    
    public void RecomputeCollision(int deleted_tile_ID)
    {
        // Update tileDeleted array considering the newly deleted tileID
        airTiles[deleted_tile_ID] = false;

        GenerateCollision2D(airTiles);
    }
}
