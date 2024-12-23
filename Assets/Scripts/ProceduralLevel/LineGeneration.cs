using UnityEngine;

/*
 * Contains the core logic to choose which tiles to use.
 */

[RequireComponent(typeof(PolygonCollider2D))]
public class LineGeneration : MonoBehaviour
{
    private GameManager game_manager;
    private int lateral_tile_amount;

    public Object tileObject;
    public int lineID = -1; // Basically, depth. We don't need to see it in Unity, but it's helpful for debugging. It could be set to readonly

    private bool[] air_tiles; // False is a tile, True is air/digged up

    private void Start()
    {
        game_manager = GameManager.Instance;
        lateral_tile_amount = game_manager.levelXSize;

        // Init Arrays
        air_tiles = new bool[lateral_tile_amount + 1]; // +1 is an additional air tile, for the collision generation logic
        air_tiles[air_tiles.Length - 1] = true; // last tile is always air

        SpawnTiles();
    }

    private void SpawnTiles() // Called once, at level start
    {
        // Generate all Tiles (digged-up tiles and air will be disabled later)
        for (int x = 0; x < lateral_tile_amount; x++)
        {
            Vector3 pos = transform.position;
            pos.x += x + 0.5f;
            pos.y -= 0.5f;

            int z_rot = Random.Range(0, 5) * 90;
            Quaternion rotation = Quaternion.Euler(0, 0, z_rot);

            GameObject new_tile = (GameObject)GameObject.Instantiate(tileObject, pos, rotation, transform);
            new_tile.GetComponent<TileScript>().xID = x;
        }

        GenerateLine(lineID);
    }

    // Called on Start and when the line is moved/reused
    public void GenerateLine(int line_ID, float seed = 0f)
    {
        // We can't use line_ID directly,
        // because it doesn't update during the same frame.
        // So, we pass it manually

        for (int x = 0; x < lateral_tile_amount; x++)
        {
            TileScript current_tile = transform.GetChild(x).GetComponent<TileScript>();

            // Update pixel_ID
            int pixel_ID = line_ID * lateral_tile_amount + x;
            current_tile.pixelID = pixel_ID;

            // Flag as air if the tile is already dug up
            if (game_manager.IsTileDugUp(pixel_ID))
                air_tiles[x] = true;
            else
                air_tiles[x] = false;

            
            // Assign Tile Type
            if (!air_tiles[x]) // If we know it's not an air tile
            {
                // Loop through all TileInfos, and place the first "valid" tile
                // Essentially, we give priority to the first types of tile.
                // See allTilesInfo.GetAllTiles() for the order.
                TileInfo selected_tile = game_manager.PixelIDToTileInfo(pixel_ID, seed);
                if (selected_tile == null)
                    Debug.LogError("Choosen tile is null !");

                current_tile.SetMaterial(selected_tile.GetMaterial());
            }

            // Disable air tile, Enable non-air tile
            current_tile.SetEnabled(!air_tiles[x]);

            // We could randomize rotation,
            // but it's not necessary when re-using the line.
            // It is already randomised on spawn.
        }

        // This might have to be updated on the next Update
        // It currently works because there is a setup in LevelGeneration.cs
        // that dirties the collision and updates it in FixedUpdate()
        GenerateCollision2D(air_tiles); // this is quite expensive, but necessary
    }

    private void GenerateCollision2D(bool[] to_generate)
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();

        Vector2[] corners = new Vector2[lateral_tile_amount + 1];

        bool prev_is_air = true;
        int half_vert_count = 0;
        for (int x = 0; x <= lateral_tile_amount; x++)
        {
            bool different = prev_is_air != to_generate[x];
            if (different) // state switched
            {
                // Make collision a bit smaller compared to the actual block
                float offset = 0.05f;
                if (!prev_is_air) // Invert offset if previous is a block (and current isn't)
                    offset *= -1;

                if (prev_is_air == true) // from air to tile
                {
                    // top-left corner
                    corners[half_vert_count].x = x + offset;
                    corners[half_vert_count].y = 0;
                }
                else // from tile to air
                {
                    // bottom-right corner
                    corners[half_vert_count].x = x + offset;
                    corners[half_vert_count].y = -1;
                }

                prev_is_air = !prev_is_air;
                half_vert_count++;
            }
        }

        // Create a new array, twice the vertex count
        // store tmpVerts inside of it, the second time with an offset of 1 in y
        Vector2[] verts = new Vector2[half_vert_count * 2];
        for (int i = 0, j = 0; j < half_vert_count; i += 4, j += 2)
        {
            // j + 0 is top-left corner
            // j + 1 is bottom-right corner
            verts[i + 0].x = corners[j + 0].x;
            verts[i + 0].y = corners[j + 0].y;

            verts[i + 1].x = corners[j + 0].x;
            verts[i + 1].y = corners[j + 1].y;

            verts[i + 2].x = corners[j + 1].x;
            verts[i + 2].y = corners[j + 1].y;

            verts[i + 3].x = corners[j + 1].x;
            verts[i + 3].y = corners[j + 0].y;
        }

        col.pathCount = half_vert_count / 2;

        Vector2[] pts = new Vector2[4];
        for (int i = 0; i < col.pathCount; i++)
        {
            pts[0] = verts[i * 4 + 0];
            pts[1] = verts[i * 4 + 1];
            pts[2] = verts[i * 4 + 2];
            pts[3] = verts[i * 4 + 3];

            col.SetPath(i, pts);
        }

        col.transform.parent.GetComponent<CompositeCollider2D>().GenerateGeometry();
    }

    public void RecomputeCollision(int deleted_pixel_ID)
    {
        // Update air_tiles array considering the newly deleted pixel_ID
        int xID = game_manager.PixelIDX(deleted_pixel_ID);
        air_tiles[xID] = true;

        GenerateCollision2D(air_tiles);
    }
}
