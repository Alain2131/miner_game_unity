using UnityEngine;
using System.Collections.Generic; // Includes List

/*
 * Refactor idea
 * Ditch the whole ProcMakeLine stuff
 * Instead, use a List of vertices
 * to tell where the center of each tile is
 * Have another List of the same length
 * for the tile type
 * Rotation can be random each time
 * For the Raycast, it could be against
 * the center vertice, if possible
 * That way, we don't have hundreds of GameObjects
 * Instead, the only expensive stuff remaining
 * will be the player collision generation
*/

/*
 * Update on refactor idea
 * Create an EmptyObject for each tile type
 * Create a script (based on drawInstancedTest)
 * It will take an array or a list as an input
 * and make a custom mesh where the tile are needed
 * 
 * Collision will have to still be generated
 * line by line, and then Composited together
 * 
 * Let's say we have Dirt and Coal
 * We'll have LevelGeneration
 * Child of that, we have 2 of the tilesObject
 * LevelGen generates the array/list of tiles
 * maybe one array for all of them, or separate arrays
 * Then, LevelGen passes it(them) to each tilesObject
 * tilesObject has a function GenerateTiles(inputArray)
 * 
 * Then, after all of that, LevelGeneration
 * takes care of the collision
 * It spawns another emptyObject with PolygonCollider2D
 * GenerateCollision() will fill that up
 * For each line, do the optimized collision
 * put all that in a massive array,
 * then set that on PolygonCollider2D
 * 
 * Three child GameObjects
 * Two for actual geometry
 * One for collision, but it's mostly separated
 * On the parent, the Composite merges them together
 */

[RequireComponent(typeof(CompositeCollider2D))]
public class LevelGeneration : MonoBehaviour
{
    public Transform player;
    public ProcMakeLine lineGeneration;

    public int ySize = 20;

    [Header("Debug")]

    // Debug variables to regenerate the level every delay seconds
    public bool Regenerate = false;
    private float tmpTime = 0f;
    public float delay = 5f;

    // Bypass expensive collision creation
    // as well as the whole MakeLine process
    // Just make an array of vertices with
    // positions where a tile would be, and
    // draw a gizmo there
    public bool preview = false; // Regenerate has to be true for this to work
    private bool preview1 = false;
    private List<Vector3> previewVerts;
    public Vector2 previewSize = new Vector2(100, 100);
    public float noiseMult = 1.5f;
    public float noiseThreshold = 0.8f;

    private List<ProcMakeLine> lines;

    private float heightThreshold;

    private bool dirtyCollision = false;

    private void Awake()
    {
        // I don't know if initializing the size is better versus simply doing List.Add()
        lines = new List<ProcMakeLine>(new ProcMakeLine[ySize]); // initialize list length
        for (int i=0; i<ySize; i++)
        {
            Vector3 pos = transform.position;
            lines[i] = GameObject.Instantiate(lineGeneration, new Vector3(pos.x, pos.y - i, pos.z), Quaternion.identity, transform);
            lines[i].lineID = i;
        }

        heightThreshold = -(ySize / 2f);
    }

    private void Update()
    {
        // Generate level as the player moves
        float height = player.position.y;
        int bufferZone = 2; // could be exposed on the script. Must be >0
        if(height > heightThreshold + bufferZone) // upper bound
        {
            if(heightThreshold >= -(ySize/2f))
                return;

            // Extract relevant lines
            ProcMakeLine firstLine = lines[0];
            ProcMakeLine currentLine = lines[lines.Count - 1];

            // Reorder List
            lines.RemoveAt(lines.Count - 1);
            lines.Insert(0, currentLine);

            // Set new position
            Vector3 newPos = firstLine.transform.position + new Vector3(0, 1, 0);
            currentLine.transform.position = newPos;

            // Update lineID
            int newID = firstLine.lineID - 1;
            currentLine.lineID = newID;

            // Generate line
            currentLine.GenerateLine(newID);

            // Recalculate collision
            dirtyCollision = true;

            // Update heightThreshold
            heightThreshold++;
        }
        else if (height < heightThreshold - bufferZone) // lower bound
        {
            // Extract relevant lines
            ProcMakeLine currentLine = lines[0];
            ProcMakeLine lastLine = lines[lines.Count-1];

            // Reorder List
            lines.RemoveAt(0);
            lines.Add(currentLine);

            // Set new position
            Vector3 newPos = lastLine.transform.position + new Vector3(0, -1, 0);
            currentLine.transform.position = newPos;
            
            // Update lineID
            int newID = lastLine.lineID + 1;
            currentLine.lineID = newID;

            // Generate line
            currentLine.GenerateLine(newID);

            // Recalculate collision
            dirtyCollision = true;

            // Update heightThreshold
            heightThreshold--;
        }
    }

    private void FixedUpdate()
    {
        if(dirtyCollision)
        {
            GetComponent<CompositeCollider2D>().GenerateGeometry();
            dirtyCollision = false;
        }

        RegenerateLevel();
    }

    private void RegenerateLevel()
    {
        if (Regenerate)
        {
            if(tmpTime == 0)
            {
                if (preview)
                {
                    if (!preview1)
                    {
                        // Hide lines
                        for (int i = 0; i < ySize; i++)
                        {
                            lines[i].gameObject.SetActive(false);
                        }

                        GeneratePreview();
                        preview1 = true;
                    }
                }
                else
                {
                    // Show lines
                    for (int i = 0; i < ySize; i++)
                    {
                        lines[i].gameObject.SetActive(true);
                    }
                    preview1 = false;

                    for (int i = 0; i < ySize; i++)
                    {
                        int ID = lines[i].lineID;
                        float seed = UnityEngine.Random.Range(0, 1000f);
                        lines[i].GenerateLine(ID, seed);
                    }
                }
            }
            
            // time management
            tmpTime += Time.deltaTime;
            if (tmpTime >= delay)
            {
                tmpTime = 0;
                previewVerts = new List<Vector3>();
                preview1 = false;
            }
        }
        else
        {
            // Show lines
            for (int i = 0; i < ySize; i++)
            {
                lines[i].gameObject.SetActive(true);
            }

            tmpTime = 0;
            previewVerts = new List<Vector3>();
            preview1 = false;
        }
    }

    private void GeneratePreview()
    {
        previewVerts = new List<Vector3>();
        int xSize = (int)previewSize[0];
        int ySize = (int)previewSize[1];

        for(int y=0; y<ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float seed = 12.456f;
                bool spawnTile = Mathf.PerlinNoise(x * noiseMult + seed, y * noiseMult + seed) > noiseThreshold;
                if(spawnTile)
                    previewVerts.Add(new Vector3(x + 0.5f, -y - 1, 0));
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (previewVerts == null)
        {
            return;
        }

        Gizmos.color = Color.gray;
        for (int i = 0; i < previewVerts.Count; i++)
        {
            Gizmos.DrawCube(previewVerts[i], new Vector3(0.9f, 0.9f, 0.1f));
        }
    }
}
