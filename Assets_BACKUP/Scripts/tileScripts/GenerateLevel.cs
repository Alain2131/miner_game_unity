using UnityEngine;
using System.Collections.Generic; // Includes List

[RequireComponent(typeof(CompositeCollider2D))]
public class GenerateLevel : MonoBehaviour
{
    public TileGeneration tileGeneration;
    public CollisionGeneration collisionGeneration;

    public int xSize = 100;
    public int ySize = 20;

    // Debug
    [Header("Debug")]
    public bool GenerateTiles = true;
    public bool GenerateCol = true;

    private string[] tileTypes = TileInfo.TileTypes;
    
    private List<int[]> allLines;
    [HideInInspector] public List<bool[]> dugTiles = new List<bool[]>();

    private TileGeneration[] tileObjects;
    private CollisionGeneration colObj;

    private void Awake()
    {
        tileObjects = new TileGeneration[tileTypes.Length];

        GenerateList();

        // Initialize Geometry Containers

        Vector3 pos = transform.position;
        // Add Tile Geometry Objects
        if (GenerateTiles)
        {
            for (int i = 0; i < tileTypes.Length; i++)
            {
                Material tileMat = Resources.Load("Materials/" + tileTypes[i] + "Mat", typeof(Material)) as Material;

                tileObjects[i] = GameObject.Instantiate(tileGeneration, pos, Quaternion.identity, transform);
                tileObjects[i].name = tileTypes[i];
                tileObjects[i].tileType = i;
                tileObjects[i].gameObject.GetComponent<Renderer>().material = tileMat;
            }
        }

        // Add Tile Collision Object
        if (GenerateCol)
        {
            colObj = GameObject.Instantiate(collisionGeneration, pos + new Vector3(-0.5f, 0.5f, 0), Quaternion.identity, transform);
            colObj.gameObject.name = "Collision";
        }

        RegenerateLevel(true, true);
    }

    void GenerateList()
    {
        allLines = new List<int[]>();
        int[] singleLine;

        dugTiles = new List<bool[]>(); // !!
        bool[] dugLine;

        int tileCount = tileTypes.Length;

        // -1 empty
        //  0 dirt
        //  1 coal
        for (int y=0; y<ySize; y++)
        {
            singleLine = new int[xSize + 1];
            dugLine = new bool[xSize + 1];
            for(int x=0; x<xSize; x++)
            {
                float noiseMult = 0.5f;
                float seed = 12.456f;
                float noiseVal = Mathf.PerlinNoise(x * noiseMult + seed, y * noiseMult + seed);
                int type = (int)(noiseVal * tileCount) % tileCount; // % tileCount to make sure we never hit the higher limit
                if (type >= tileCount || type < 0)
                    print("weird type");

                if (noiseVal < 0.2f) // make holes
                    dugLine[x] = true; // true
                if (y == 0) // force first line to be dirt
                {
                    type = 0;
                    dugLine[x] = false;
                }

                singleLine[x] = type;
            }
            // Add an empty square at the end of the line
            // This will ensure the collision is "closed" after each line
            singleLine[xSize] = 0;
            dugLine[xSize] = true;

            allLines.Add(singleLine);
            dugTiles.Add(dugLine);
        }
    }

    public void RegenerateLevel(bool doTiles, bool doCol)
    {
        // Generate Tiles
        if (doTiles)
        {
            for (int i = 0; i < tileTypes.Length; i++)
            {
                tileObjects[i].GenerateTiles(allLines, dugTiles);
            }
        }
        
        if (doCol)
        {
            // Generate Collision
            colObj.GenerateCollision(allLines, dugTiles);
        }
    }

    public void DigTile(Vector2 TileID)
    {
        //print("DigTile");
        int x = (int)TileID[0];
        int y = (int)TileID[1];
        dugTiles[y][x] = true;
        RegenerateLevel(true, true);
    }
}
