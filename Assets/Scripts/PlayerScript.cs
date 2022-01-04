using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D rb;

    public float upForce = 1250f;
    public float lateralSpeed = 1000f;

    void Update()
    {
        float dtime = Time.deltaTime;

        // Make sure the player can't move faster than a certain speed
        float maxSpeed = 20f;
        float playerSpeed = rb.velocity.magnitude;
        if(playerSpeed > maxSpeed)
        {
            float mult = maxSpeed / playerSpeed;
            rb.velocity = new Vector2(rb.velocity.x * mult, rb.velocity.y * mult);
        }


        // Player Movement
        // I may have to split that into another script
        // Move Up
        if(Input.GetKey("w") || Input.GetKey("space") || Input.GetKey("up"))
        {
            rb.AddForce(new Vector3(0, upForce * dtime, 0));
        }

        // Move Left
        if(Input.GetKey("a") || Input.GetKey("left"))
        {
            rb.AddForce(new Vector3(-lateralSpeed * dtime, 0, 0));

            if (isPlayerOnTile())
            {
                Dig(Vector3.left);
            }
        }

        // Move Right
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            rb.AddForce(new Vector3(lateralSpeed * dtime, 0, 0));

            if (isPlayerOnTile())
            {
                Dig(Vector3.right);
            }
        }

        // Please make sure we are on the ground first
        // Dig Down
        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            if (isPlayerOnTile()) // this check isn't really necessary for digging down
            {
                Dig(Vector3.down);
            }
            else if (isPlayerOnFloor()) // This is janky
            {
                RaycastHit2D hit = isPlayerOnFloor();
                Transform floor = hit.transform;
                floor.GetComponent<Floor>().Disable();
            }
        }
    }

    private RaycastHit2D isPlayerOnFloor()
    {
        RaycastHit2D hit = PlayerRaycast(Vector3.down * 0.475f, "floor", false);
        return hit;
    }

    private RaycastHit2D PlayerRaycast(Vector3 rayDirection, string layerName = "tile", bool debugDraw = false)
    {
        int layerMask = LayerMask.GetMask(layerName);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, rayDirection.magnitude, layerMask);

        if (debugDraw)
        {
            if (hit.collider != null)
                Debug.DrawRay(transform.position, rayDirection.normalized * hit.distance, Color.green);
            else
                Debug.DrawRay(transform.position, rayDirection, Color.red);
        }
        
        return hit;
    }

    private bool isPlayerOnTile()
    {
        RaycastHit2D hit = PlayerRaycast(Vector3.down * 0.475f, "tile", false);
        return hit.collider != null;
    }

    private void Dig(Vector3 DigDirection)
    {
        // Fetch Tile
        RaycastHit2D hit = PlayerRaycast(DigDirection * 0.6f, "tile", false);

        if (hit.collider != null)
        {
            hit.collider.GetComponent<TileScript>().DigTile();

            // Fetch Tile Information
            // what type is it, how long to dig it, its weight, etc

            // Update various stuff
            // inventory

            // Move Player to tile "center", but keeping the original height (only move laterally)
            // with an offset in y of maybe 0.05f
            // (based on the player being 0.9, and the tile being 1),
            // to keep the player on the ground

            // Maybe partially disable player input during that time
            // leave Pausing on, but movement should be disabled
        }
    }
}
