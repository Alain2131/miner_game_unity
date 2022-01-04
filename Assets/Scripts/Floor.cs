using UnityEngine;

// An invisible collider with the size of the level's width,
// allowing the player to move on the surface with no tile under it,
// until he presses down to go through.

// This works for the first floor.
// If we need multiple floor levels, then some tweaking
// might need to be done (such as the player position)
// But if we transition to the trigger method, then we might be good.

// Important notice - The GameObject has to be in the "floor" Layer.

[RequireComponent(typeof(BoxCollider2D))]
public class Floor : MonoBehaviour
{
    private GameManager gameManager;
    private int xSize;
    new private BoxCollider2D collider;

    private void Start()
    {
        gameManager = GameManager.Instance;
        xSize = gameManager.LevelXSize;
        collider = transform.GetComponent<BoxCollider2D>();

        // Set the collider to the right size
        Vector2 offset = new Vector2(xSize * 0.5f, -0.5f);
        Vector2 size = new Vector2(xSize, 1);
        collider.offset = offset;
        collider.size = size;
    }

    private void Update()
    {
        // This is really not ideal, and just for a temporary "it works"

        // What I envision is two triggers.
        // trigger1 has the same size as the collider.
        // trigger2 is very thin in height, but same width as the collider,
        // and is placed on top of the collider, without intersecting with it.

        // If we stop overlapping with trigger1 and trigger2 is overlapping with the player,
        // enable the floor.
        if(gameManager.player.transform.position.y > 0.5f)
        {
            Enable();
        }
    }

    public void Enable()
    {
        if(!collider.enabled)
            collider.enabled = true;
    }

    public void Disable()
    {
        collider.enabled = false;
    }
}
