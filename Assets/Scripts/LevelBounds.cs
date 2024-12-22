using UnityEngine;

// Two invisible colliders on both edges of the level (in width)
// moving vertically with the player to make sure he can't go outside the level.

// Notice - The GameObject can be in the "Ignore Raycast" Layer.

public class LevelBounds : MonoBehaviour
{
    private GameManager game_manager;
    private int x_size;
    private BoxCollider2D collider1;
    private BoxCollider2D collider2;
    private Transform player_transform;

    // Automatically add two BoxCollider2D
    // (because RequireComponent can only add one)
    void Reset()
    {
        int num_colliders = GetComponents<BoxCollider2D>().Length;
        for (int i = 0; i < 2 - num_colliders; i++)
            gameObject.AddComponent<BoxCollider2D>();
    }

    void Start()
    {
        game_manager = GameManager.Instance;
        x_size = game_manager.levelXSize;
        collider1 = GetComponents<BoxCollider2D>()[0];
        collider2 = GetComponents<BoxCollider2D>()[1];
        player_transform = game_manager.player.transform;

        // Set sizes and offsets
        Vector2 offset = new Vector2(-0.5f, 0);
        Vector2 size = new Vector2(1, 10); // tall boi
        
        collider1.size = size;
        collider2.size = size;

        collider1.offset = offset;
        offset.x = x_size + 0.5f;
        collider2.offset = offset;
    }

    void Update()
    {
        // Move with player
        Vector3 pos = player_transform.position;
        pos.x = 0;
        pos.z = 0;
        transform.position = pos;
    }
}
