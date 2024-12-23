using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class TilesCollisionAroundPlayer : MonoBehaviour
{
    private GameManager game_manager;
    private Transform player_transform;

    private PolygonCollider2D polygon_collider_2d;

    void Start()
    {
        game_manager = GameManager.Instance;
        player_transform = game_manager.player.transform;
        polygon_collider_2d = transform.GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        int player_pixel_ID = game_manager.PositionToPixelID(player_transform.position);
        //Debug.Log($"player_pixel_ID {player_pixel_ID}");

        // Most of this code was copied from WorldGeneration_Collision.cs
        float tile_size = game_manager.GetPixelWorldSize();
        float half = tile_size * 0.5f;

        Vector2[] corners = new Vector2[4];
        Vector2 center;

        int path_counter = -1;
        for (int y_offset = -1; y_offset <= 1; y_offset++)
        {
            for (int x_offset = -1; x_offset <= 1; x_offset++)
            {
                // skip center tile, where the player is
                if (x_offset == 0 && y_offset == 0)
                    continue;
                
                path_counter++;

                int pixel_ID = game_manager.GetPixelIDAtOffset(player_pixel_ID, x_offset, y_offset);

                bool is_air = game_manager.IsTileDugUp(pixel_ID);
                float mult = is_air ? 0.0f : 1.0f;
                
                center = game_manager.PixelIDToPosition(pixel_ID);
                corners[0] = center + new Vector2(-half, half) * mult;
                corners[1] = center + new Vector2(half, half) * mult;
                corners[2] = center + new Vector2(half, -half) * mult;
                corners[3] = center + new Vector2(-half, -half) * mult;
                polygon_collider_2d.SetPath(path_counter, corners);
            }
        }
    }
}
