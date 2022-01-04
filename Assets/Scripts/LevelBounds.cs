using UnityEngine;

// Two invisible colliders on both edges of the level (in width)
// moving vertically with the player to make sure he can't go outside the level.

// Notice - The GameObject can be in the "Ignore Raycast" Layer.

public class LevelBounds : MonoBehaviour
{
    private GameManager gameManager;
    private int xSize;
    private BoxCollider2D collider1;
    private BoxCollider2D collider2;

    // Automatically add two BoxCollider2D
    // (because RequireComponent can only add one)
    void Reset()
    {
        int numColliders = GetComponents<BoxCollider2D>().Length;
        for (int i = 0; i < 2 - numColliders; i++)
            gameObject.AddComponent<BoxCollider2D>();
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        xSize = gameManager.LevelXSize;
        collider1 = GetComponents<BoxCollider2D>()[0];
        collider2 = GetComponents<BoxCollider2D>()[1];

        // Set sizes and offsets
        Vector2 offset = new Vector2(-0.5f, 0);
        Vector2 size = new Vector2(1, 10); // tall boi
        
        collider1.size = size;
        collider2.size = size;

        collider1.offset = offset;
        offset.x = xSize + 0.5f;
        collider2.offset = offset;
    }

    void Update()
    {
        // Move with player
        Vector3 pos = gameManager.player.transform.position;
        pos.x = 0;
        pos.z = 0;
        transform.position = pos;
    }
}
