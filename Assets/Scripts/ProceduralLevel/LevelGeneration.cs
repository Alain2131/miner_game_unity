using UnityEngine;
using System.Collections.Generic; // Includes List

[RequireComponent(typeof(CompositeCollider2D))]
public class LevelGeneration : MonoBehaviour
{
    public Transform player;
    public LineGeneration lineGeneration;

    public int ySize = 20;

    private List<LineGeneration> lines;

    private float heightThreshold;

    private bool dirtyCollision = false;

    private void Awake()
    {
        // I don't know if initializing the size is better versus simply doing List.Add()
        lines = new List<LineGeneration>(new LineGeneration[ySize]); // initialize list length
        for (int i=0; i<ySize; i++)
        {
            Vector3 pos = transform.position;
            lines[i] = GameObject.Instantiate(lineGeneration, new Vector3(pos.x, pos.y - i, pos.z), Quaternion.identity, transform);
            lines[i].lineID = i;
        }

        heightThreshold = -(ySize / 2f);

        TileInfo a = AllTiles.Dirt();
        print(a.Value());
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
            LineGeneration firstLine = lines[0];
            LineGeneration currentLine = lines[lines.Count - 1];

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
            LineGeneration currentLine = lines[0];
            LineGeneration lastLine = lines[lines.Count-1];

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
    }
}
