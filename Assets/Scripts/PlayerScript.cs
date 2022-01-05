using System.Threading.Tasks;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody2D rb;

    public float upForce = 1250f;
    public float lateralSpeed = 1000f;

    [Header("Debug")]
    [SerializeField] private bool DisableDigAnimation = false;

    private bool isDigging = false;

    void Update()
    {
        ClampPlayerSpeed();

        if(!isDigging)
            HandlePlayerInput();
    }

    private void ClampPlayerSpeed()
    {
        // Make sure the player can't move faster than a certain speed
        // Especially useful when falling
        float maxSpeed = 20f;
        float playerSpeed = rb.velocity.magnitude;
        if (playerSpeed > maxSpeed)
        {
            float mult = maxSpeed / playerSpeed;
            rb.velocity = new Vector2(rb.velocity.x * mult, rb.velocity.y * mult);
        }
    }

    private void HandlePlayerInput()
    {
        float dtime = Time.deltaTime;

        // Player Movement
        // I may have to split that into another script
        // Move Up
        if (Input.GetKey("w") || Input.GetKey("space") || Input.GetKey("up"))
        {
            rb.AddForce(new Vector3(0, upForce * dtime, 0));
        }

        // Move Left
        if (Input.GetKey("a") || Input.GetKey("left"))
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

            RaycastHit2D hit = isPlayerOnFloor();
            if (hit)
            {
                Transform floor = hit.transform;
                floor.GetComponent<Floor>().Disable();
            }
        }

        // Building Interact
        if (Input.GetKeyDown("e"))
        {
            if (isPlayerOnFloor()) // There will always be a floor under a building
            {
                CheckInteraction();
            }
        }
    }

    // "Dig" isn't really what this does, this is just in-between the input and the actual dig
    private void Dig(Vector3 DigDirection)
    {
        // Fetch Tile
        RaycastHit2D hit = PlayerRaycast(DigDirection * 0.6f, "tile", false);

        if (hit.collider != null)
        {
            if (!isDigging)
            {
                // Make sure we didn't just bounce
                // This might have to be handled with a PlayerState
                // As long as we haven't been on the ground for X amount of time,
                // we are not allowed to dig (Falling State)
                // if (playerState.onGround) doDig();
                TileScript tile = hit.collider.GetComponent<TileScript>();
                if (DisableDigAnimation)
                    tile.DigTile();
                else
                    DigAnimation(tile);
            }
        }
    }

    // I wasn't able to make an IEnumerator work, so I used async instead.
    private async void DigAnimation(TileScript tile)
    {
        //Debug.Log("Diggy diggy hole");
        isDigging = true;

        // We'll have to make sure to not dig if the player just took damage from falling,
        // we don't want to cancel the bounce nor be able to dig at that moment.
        rb.velocity = Vector2.zero;
        rb.simulated = false; // Disable RigidBody when digging

        float digTime = tile.tileInfo.GetDigTime();

        // The positions aren't exactly right
        // The target position is in the middle of the tile.
        // That results in the player being off the ground at the end.
        // A solution would be to place the pivots
        // of the player and the tiles at the bottom middle.
        // Another solution would be to calculate the offset
        // and add that to the targetPosition, but that's more janky.
        Vector3 currentPos = transform.position;
        Vector3 targetPosition = tile.transform.position;

        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / digTime;
            transform.position = Vector3.Lerp(currentPos, targetPosition, t);
            transform.position += AddJitter();

            int wait = (int)(Time.deltaTime * 1000);
            await Task.Delay(wait);
        }

        tile.DigTile();

        rb.simulated = true;
        isDigging = false;
    }

    private Vector3 AddJitter()
    {
        // The Jitter starts and stops abruptly
        // It would be nice to have an easein and easeout
        // over like 0.1 seconds

        // You can make those public at the top to tweak them
        float jitterIntensity = 0.1f;
        float jitterScale = 15f;

        float time = Time.realtimeSinceStartup;

        Vector3 jitterOffset;
        jitterOffset.x = Mathf.PerlinNoise((time + 12.4f) * jitterScale, 0f) - 0.5f;
        jitterOffset.y = Mathf.PerlinNoise((time + 9.647f) * jitterScale, 0f) - 0.5f;
        jitterOffset.z = 0;

        jitterOffset *= jitterIntensity;

        return jitterOffset;
    }

    // Utility methods
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

    // OnTile and OnFloor are basically the same, I could make a single method for those two
    private bool isPlayerOnTile()
    {
        RaycastHit2D hit = PlayerRaycast(Vector3.down * 0.475f, "tile", false);
        return hit.collider != null;
    }

    private RaycastHit2D isPlayerOnFloor()
    {
        RaycastHit2D hit = PlayerRaycast(Vector3.down * 0.475f, "floor", false);
        return hit;
    }

    private void CheckInteraction() // Interact with Building, for the time being
    {
        RaycastHit2D hit = PlayerRaycast(Vector3.forward, "building", false);

        if (hit)
        {
            if (hit.transform.GetComponent<Interactable>())
            {
                hit.transform.GetComponent<Interactable>().Interact();
            }
        }
    }
}
