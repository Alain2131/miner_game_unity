using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class WorldGeneration_Collision : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;
    public WorldGeneration worldGeneration;

    private Texture2D world_tex2D;
    private PolygonCollider2D collider_2D;

    private int oldLocationID;

    void Awake()
    {
        world_tex2D = worldGeneration.tex2D;
        collider_2D = transform.GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        int locationID = worldGeneration.PositionToPixelID(playerPosition.position);
        if(locationID != oldLocationID)
        {
            oldLocationID = locationID;

            ClearCollisions(); // Maybe this should be in GenerateCollision() since we never want to generate without clearing first
            GenerateCollision(locationID);
        }
    }

    private void ClearCollisions()
    {
        Debug.Log(collider_2D.points);
        //collider_2D.points = new Vector2[6];
    }

    private void GenerateCollision(int locationID)
    {
        // Example in
        // LineGeneration.cs -> GenerateCollision2D()


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
        // 
    }
}
