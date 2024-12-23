using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(CompositeCollider2D))]
public class WorldGeneration_Collision : MonoBehaviour
{
    [SerializeField] private Transform player;
    public WorldGeneration worldGeneration;
    [Tooltip("Amount of tiles of collision generated around the player.")]
    public int collisionPadding = 2;
    public GameObject square;

    //private Texture2D world_tex2D;
    private PolygonCollider2D collider_2D;
    private CompositeCollider2D composite_collider_2D;
    private List<GameObject> squaresPool = new List<GameObject>();

    private int oldLocationID;

    void Awake()
    {
        //world_tex2D = worldGeneration.tex2D;
        collider_2D = transform.GetComponent<PolygonCollider2D>();
        composite_collider_2D = transform.GetComponent<CompositeCollider2D>();
    }

    private void Start()
    {
        float tile_size = worldGeneration.GetPixelWorldSize();
        square.transform.localScale = new Vector3(tile_size, tile_size, tile_size);
    }

    void Update()
    {
        int location_ID = worldGeneration.PositionToPixelID(player.position);
        if(location_ID != oldLocationID)
        {
            oldLocationID = location_ID;

            GenerateCollision(location_ID);
            //GenerateCollision_alt(locationID);
        }
    }

    // 4 or more collisionPadding have noticeable performance hit when generated
    // 10 or more is really bad
    // The expensive part is setting up the collider2D
    private void GenerateCollision(int location_ID)
    {
        // Example in
        // LineGeneration.cs -> GenerateCollision2D()

        // Add a square collision for each non-air tiles around the player
        // Then merge the collisions together using the Composite Collider 2D

        float tile_size = worldGeneration.GetPixelWorldSize();
        float half = tile_size * 0.5f;

        int size = (collisionPadding * 2) - 1;
        collider_2D.pathCount = size * size;

        Vector2[] corners = new Vector2[4];
        int counter = 0;
        int skipped_count = 0;
        for (int x_offset = -size / 2; x_offset <= size / 2; x_offset++)
        {
            for (int y_offset = -size / 2; y_offset <= size / 2; y_offset++)
            {
                int pixel_ID = worldGeneration.GetPixelAtOffset(location_ID, x_offset, y_offset);
                if(pixel_ID == -1)
                {
                    skipped_count++;
                    continue;
                }
                //worldGeneration.CreateQuadAtPixelID(pixelID);

                Color color = worldGeneration.SampleAtID(pixel_ID, false);

                if(color.r == 0) // skip if air
                {
                    skipped_count++;
                    continue;
                }


                Vector2 center = worldGeneration.PixelIDToPosition(pixel_ID);
                corners[0] = center + new Vector2(-half, half);
                corners[1] = center + new Vector2(half, half);
                corners[2] = center + new Vector2(half, -half);
                corners[3] = center + new Vector2(-half, -half);
                collider_2D.SetPath(counter, corners);

                counter++;
            }
        }

        // Skipping leaves a "gap" at the end, this accounts for that
        collider_2D.pathCount = (size * size) - skipped_count;

        // Handles merging the tiles together
        composite_collider_2D.GenerateGeometry();


        // Here is an earlier thought process about figuring out an algorithm
        // to get an "optimized" collision, i.e. with a continuous edge,
        // instead of discrete tiles. The issue with that idea is its inability to
        // have holes - but it's not necessarily a problem.


        // Generate collision around player location
        // in a radius of X tiles (1 or 2, perhaps)

        // Start with an arbitrary corner (i.e. top-left)
        // if air, continue;
        // if tile, we'll want to expand to all connected tiles.
        // if there is no tile to the right, add two points at the top.
        // if there is a tile to the right, add a single point at the top-left
        // then, expand, and based on adjacent tiles, add relevant points
        // will have to figure out the proper logic/algorithm
        // Iterate until no more tiles are available or until air has been hit everywhere
        // at each iteration, store the state of each tiles within an array initialized to -1s
        // so that when a patch is done, we can start a new patch with the first tile that's -1

        // "expand" can be to go all the way down (until air or collision radius)
        // or all the way right, or whatever, pick a lane and stick to it
        // it's not impossible that we're required to expand up,
        // it's not only down and right.
        // Because of that, maybe it'd be better to always check the four directions
        // if we already visited, good, next side (maybe we can ignore the direction we just came from, tho)
    }

    // This was an attempt to get the performance to be better
    // But no, it's basically the same
    private void GenerateCollision_alt(int location_ID)
    {
        // Example in
        // LineGeneration.cs -> GenerateCollision2D()
        //Debug.Log(locationID);

        // Add a square collision for each non-air tiles around the player
        // Then merge the collisions together using the Composite Collider 2D

        int size = (collisionPadding * 2) - 1;

        int counter = 0;
        //int skippedCount = 0;
        for (int x_offset = -size / 2; x_offset <= size / 2; x_offset++)
        {
            for (int y_offset = -size / 2; y_offset <= size / 2; y_offset++)
            {
                int pixel_ID = worldGeneration.GetPixelAtOffset(location_ID, x_offset, y_offset);
                if (pixel_ID == -1)
                {
                    continue;
                }
                //worldGeneration.CreateQuadAtPixelID(pixelID);

                Color color = worldGeneration.SampleAtID(pixel_ID);

                if (color.r == 0) // skip if air
                {
                    continue;
                }


                Vector2 center = worldGeneration.PixelIDToPosition(pixel_ID);

                if (counter < squaresPool.Count)
                {
                    squaresPool[counter].transform.position = center;
                    squaresPool[counter].SetActive(true);
                }
                else
                {
                    // what's "g"
                    GameObject g = Instantiate(square, center, Quaternion.identity);
                    g.transform.SetParent(transform);

                    squaresPool.Add(g);
                }

                counter++;
            }
        }

        // disable overflow
        for (int i = counter; i < squaresPool.Count; i++)
        {
            squaresPool[i].SetActive(false);
        }

        // Has an issue where the generation is a bit too soon,
        //composite_collider_2D.GenerateGeometry();
        // it's "one iteration" behind
        // manually clicking Regenerate Collider works

        // So instead, we leave the engine to generate the collision "when it needs to"
        composite_collider_2D.generationType = CompositeCollider2D.GenerationType.Synchronous;
    }

    // Another solution would be to use a MeshCollider
    // I think that generating the mesh would not produce a hit in performance
    // And if it does, we can offload the array generation to a Compute Shader
    // Here's the issues I think we'll have
    // 1. We can't "merge" the collisions like the CompositeCollider2D,
    // each triangles will be a discrete triangle.
    // I think that when sliding along the floor/on a shaft wall,
    // we'll hit the "gaps" in-between the triangles.
    // 2. We're in 2D, how does a non-2D collision type interact with 2D stuff ?
}
