using UnityEngine;
using System.Collections.Generic; // Includes List

[RequireComponent(typeof(PolygonCollider2D))]
public class CollisionGeneration : MonoBehaviour
{
    public void GenerateCollision(List<int[]> allLines, List<bool[]> dugTiles)
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();

        int xSize = allLines[0].Length;

        List<Vector2> corners = new List<Vector2>();

        bool prev = false;
        int halfVertCount = 0;
        for(int y=0; y<allLines.Count; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                bool toGenerate = !dugTiles[y][x];
                bool same = !(prev ^ toGenerate);
                if (!same) // state switched
                {
                    // Make collision a bit smaller compared to the actual block
                    float offset = 0.00f; // 0.05f
                    if (prev) // Invert offset if previous is a block (and current isn't)
                        offset *= -1;

                    corners.Add(new Vector2());

                    if (prev == false) // from empty to tile
                        corners[halfVertCount] = new Vector2(x + offset, 0 - y); // top-left corner
                    else // from tile to empty
                        corners[halfVertCount] = new Vector2(x + offset, -1 - y); // bottom-right corner

                    prev = !prev;
                    halfVertCount++;
                }
            }
        }

        // Create a new array, twice the vertex count
        // store tmpVerts inside of it, the second time with an offset of 1 in y
        Vector2[] verts = new Vector2[halfVertCount * 2];
        for (int i = 0, j = 0; j < halfVertCount; i += 4, j += 2)
        {
            // j   is top-left corner
            // j+1 is bottom-right corner
            float a = 0.001f;
            verts[i + 0] = new Vector2(corners[j].x, corners[j].y-a);
            verts[i + 1] = new Vector2(corners[j].x, corners[j + 1].y+a);
            verts[i + 2] = new Vector2(corners[j + 1].x, corners[j + 1].y+a);
            verts[i + 3] = new Vector2(corners[j + 1].x, corners[j].y-a);
        }

        col.pathCount = halfVertCount / 2;

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




    private void GenerateCollision2D(bool[] toGenerate)
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();

        int xSize = 100;

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

                if (prev == false) // from empty to tile
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
        for (int i = 0, j = 0; j < halfVertCount; i += 4, j += 2)
        {
            // j   is top-left corner
            // j+1 is bottom-right corner
            verts[i + 0] = new Vector2(corners[j].x, corners[j].y);
            verts[i + 1] = new Vector2(corners[j].x, corners[j + 1].y);
            verts[i + 2] = new Vector2(corners[j + 1].x, corners[j + 1].y);
            verts[i + 3] = new Vector2(corners[j + 1].x, corners[j].y);
        }

        col.pathCount = halfVertCount / 2;

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
}
