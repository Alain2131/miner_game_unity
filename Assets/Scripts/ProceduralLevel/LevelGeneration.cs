using UnityEngine;
using System.Collections.Generic; // Includes List

[RequireComponent(typeof(CompositeCollider2D))]
public class LevelGeneration : MonoBehaviour
{
    private GameManager game_manager;
    private int y_size;

    public LineGeneration lineGeneration;

    private List<LineGeneration> lines;
    private float height_threshold;
    private bool dirty_collision = false;

    private void Start()
    {
        game_manager = GameManager.Instance;
        y_size = game_manager.levelYSize;

        // I don't know if initializing the size is better versus simply doing List.Add()
        lines = new List<LineGeneration>(new LineGeneration[y_size]); // initialize list length
        for (int i=0; i<y_size; i++)
        {
            Vector3 pos = transform.position;
            pos.y -= i;
            lines[i] = GameObject.Instantiate(lineGeneration, pos, Quaternion.identity, transform);
            lines[i].lineID = i;
        }

        height_threshold = -(y_size / 2f);
    }

    private void Update()
    {
        UpdateLevelHeight();
    }

    private void FixedUpdate()
    {
        if(dirty_collision)
        {
            GetComponent<CompositeCollider2D>().GenerateGeometry();
            dirty_collision = false;
        }
    }

    private void UpdateLevelHeight()
    {
        // Generate level as the player moves vertically
        float height = game_manager.player.position.y;
        int buffer_zone = 2; // could be exposed on the script. Must be >0

        if (height > height_threshold + buffer_zone) // upper bound
        {
            if (height_threshold >= -(y_size / 2f))
                return;

            // Extract relevant lines
            LineGeneration first_line = lines[0];
            LineGeneration current_line = lines[lines.Count - 1];

            // Reorder List
            lines.RemoveAt(lines.Count - 1);
            lines.Insert(0, current_line);

            // Set new position
            Vector3 new_pos = first_line.transform.position;
            new_pos.y += 1;
            current_line.transform.position = new_pos;

            // Update lineID
            int new_ID = first_line.lineID - 1;
            current_line.lineID = new_ID;

            // Generate line
            current_line.GenerateLine(new_ID);

            // Recalculate collision
            dirty_collision = true;

            // Update heightThreshold
            height_threshold++;
        }
        else if (height < height_threshold - buffer_zone) // lower bound
        {
            // Extract relevant lines
            LineGeneration current_line = lines[0];
            LineGeneration last_line = lines[lines.Count - 1];

            // Reorder List
            lines.RemoveAt(0);
            lines.Add(current_line);

            // Set new position
            Vector3 new_pos = last_line.transform.position;
            new_pos.y -= 1;
            current_line.transform.position = new_pos;

            // Update lineID
            int new_ID = last_line.lineID + 1;
            current_line.lineID = new_ID;

            // Generate line
            current_line.GenerateLine(new_ID);

            // Recalculate collision
            dirty_collision = true;

            // Update heightThreshold
            height_threshold--;
        }
    }
}
