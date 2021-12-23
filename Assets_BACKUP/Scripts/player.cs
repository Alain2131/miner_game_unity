using UnityEngine;

public class player : MonoBehaviour
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

            if (isPlayerOnGround())
            {
                Dig(Vector3.left);
            }
        }

        // Move Right
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            rb.AddForce(new Vector3(lateralSpeed * dtime, 0, 0));

            if (isPlayerOnGround())
            {
                Dig(Vector3.right);
            }
        }

        // Please make sure we are on the ground first
        // Dig Down
        if (Input.GetKey("s") || Input.GetKey("down"))
        {
            if (isPlayerOnGround()) // this check isn't really necessary for digging down
            {
                // Maybe we could do a fetchTile() function first
                Dig(Vector3.down);
            }
        }
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

    private void Dig(Vector3 DigDirection)
    {
        // Fetch Tile
        RaycastHit2D hit = PlayerRaycast(DigDirection * 0.6f, "tile", true);

        if (hit.collider != null)
        {
            Vector2 tileID = GetTileIDFromCollision(hit);

            //print(tileID);

            //hit.collider.gameObject.GetComponent<GenerateLevel>().DigTile(tileID);
            //hit.collider.transform.parent.gameObject.GetComponent<GenerateLevel>().DigTile(tileID);

            //GameObject tile = hit.collider.gameObject;
            //print(tile);
            //GameObject parent = tile.transform.parent.gameObject;
            //print(parent);

            // The ID will tell the Collision Generator which tile it should remove
            //int tileID = tile.GetComponent<tile>().ID;
            //parent.GetComponent<ProcMakeLine>().RecomputeCollision(tileID);

            GameObject levelGen = hit.collider.gameObject;
            //levelGen.GetComponent<ProcMakeLine>().RecomputeCollision(tileID);

            // We'll need to move the player to the center of the tile
            //Vector3 tilePos = tile.transform.position;

            // Instead of deleting the tile, we disable it
            // When reusing the line, we can just re-enable the tiles
            // and change their type
            hit.collider.gameObject.GetComponent<tile>().DisableTile();
        }

        // Fetch Tile Information
        // what type is it, how long to dig it, its weight, etc

        // Update various stuff
        // inventory

        // Move Player to tile "center"
        // with an offset in y of maybe 0.05f
        // (based on the player being 0.9, and the tile being 1),
        // to keep the player on the ground

        // Maybe partially disable player input during that time
        // leave Pausing on, but movement should be disabled
    }

    private Vector2 GetTileIDFromCollision(RaycastHit2D hit)
    {
        Vector2 dir = hit.normal * -1; // We get the inverse of the normal
        Vector2 center = hit.point + (dir * 0.5f);

        // Make sure the center is integer
        int x = (int)Mathf.Round(center[0]);
        int y = (int)Mathf.Round(center[1]);

        x = Mathf.Abs(x);
        y = Mathf.Abs(y);

        Vector2 TileID = new Vector2(x, y);
        //Debug.DrawRay(TileID, Vector2.up * 0.1f, Color.white);

        return TileID;
    }

    private bool isPlayerOnGround()
    {
        RaycastHit2D hit = PlayerRaycast(Vector3.down * 0.475f, "tile", false);
        return hit.collider != null;
    }
}
