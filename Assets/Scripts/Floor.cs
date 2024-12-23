using UnityEngine;

// An invisible collider with the size of the level's width,
// allowing the player to move on the surface with no tile under it,
// until he presses down to go through.

// This works for the first floor.
// If we need multiple floor levels, then some tweaking
// might need to be done (such as the player position)
// But if we transition to the trigger method, then we might be good.

// Important notice - The GameObject has to be in the "floor" Layer.

// The trigger doesn't need to be an EdgeCollider2D, could be a BoxCollider2D,
// but I don't know how to require both of the same type
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Floor : MonoBehaviour
{
    private GameManager game_manager;
    private int x_size;
    new private BoxCollider2D collider;
    private EdgeCollider2D trigger;

    private Transform player_transform;

    private void Start()
    {
        game_manager = GameManager.Instance;
        x_size = game_manager.levelXSize;
        collider = transform.GetComponent<BoxCollider2D>();
        trigger = transform.GetComponent<EdgeCollider2D>();
        player_transform = game_manager.player.transform;

        // Set the collider to the right size
        Vector2 offset = new Vector2(x_size * 0.5f, -0.5f);
        Vector2 size = new Vector2(x_size, 1);
        collider.offset = offset;
        collider.size = size;

        // Set the trigger to the right size
        trigger.isTrigger = true;
        trigger.edgeRadius = 0.5f;
        Vector2[] points = { new Vector2(0, -1.5f), new Vector2(x_size, -1.5f) };
        trigger.points = points;
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
        if(player_transform.position.y > 0.5f)
        {
            Enable();
        }

        // If the player somehow gets under the floor when it's disabled,
        // this will allow them to go over ground.
        if (trigger.IsTouching(player_transform.GetComponent<BoxCollider2D>()))
            Disable();
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
