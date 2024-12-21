using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Rigidbody2D.IsTouchingLayers

public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody2D rb;

    public float upForce = 1250f;
    public float lateralSpeed = 1000f;
    public float propellerMultiplier = 1.0f;

    [Header("Hull Strength")]
    public int maxHealth = 100;
    public int currentHealth;
    public SliderBar hullBar;
    public HurtOverlay hurtOverlay;
    public bool omnidirectionalDamage = true;

    [Header("Fuel")]
    public int maxFuel = 100;
    public float currentFuel;
    public SliderBar fuelBar;

    [Header("Drill")]
    public float drillSpeed = 1.0f;

    [Header("Cargo")]
    public int cargoSize = 1000;

    [Header("Debug")]
    [SerializeField] private bool disableDigAnimation = false;
    [SerializeField] private bool disableDamage = false;
    [SerializeField] private bool disableFuel = false;

    [Header("Items")]
    public int itemExplosiveCount = 5;

    private GameManager gameManager;

    private bool isDigging = false;
    private bool isFalling = false; // flying as well

    private float previousFrameSpeed = 0;


    private Controls controls;
    private void Awake()
    {
        gameManager = GameManager.Instance;

        // Bind controls Callbacks
        // rebind -> https://youtu.be/Yjee_e4fICc?t=2478
        // save rebind -> https://youtu.be/Yjee_e4fICc?t=2573
        // mobile (on-screen) controls -> https://youtu.be/Yjee_e4fICc?t=2578

        controls = gameManager.controls;
        controls.Gameplay.Enable();
        controls.Gameplay.Inventory.performed += ToggleInventory;
        controls.Gameplay.TogglePause.performed += TogglePause;
        controls.Gameplay.Interact.performed += Interact;
        controls.Gameplay.Explosive.performed += Item_Explosive;

        controls.MenuControls.Cancel.performed += TogglePause;
        controls.MenuControls.Interact.performed += Interact;
    }


    private void Start()
    {
        currentHealth = maxHealth;
        hullBar.SetMaxValue(maxHealth);

        currentFuel = maxFuel;
        fuelBar.SetMaxValue(maxFuel);

        // Constant Fuel Consumption, always active
        StartCoroutine("FuelConsumption", 3); // Remove 1 fuel over X seconds


        //gameManager.CreateQuadAtPixelID(5);
        //gameManager.CreateQuadAtPixelID(101);

        /*gameManager.CreateQuadAtPixelID(0);
        gameManager.CreateQuadAtPixelID(6);
        gameManager.CreateQuadAtPixelID(25);
        gameManager.CreateQuadAtPixelID(50);
        gameManager.CreateQuadAtPixelID(75);
        gameManager.CreateQuadAtPixelID(99);
        gameManager.CreateQuadAtPixelID(100);
        gameManager.CreateQuadAtPixelID(106);*/
    }


    private void FixedUpdate()
    {
        ClampPlayerSpeed();

        HandleInputs();

        isFalling = !IsPlayerOnGround();

        HandleFallDamage();
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        gameManager.oreInventory.ToggleInventoryUI();
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        gameManager.TogglePauseGame();
    }

    private void Interact(InputAction.CallbackContext context)
    {
        // Allow for interaction only when the game is not paused
        // Much like gameManager.TogglePauseGame(), this feels like a hack.
        if (gameManager.gamePaused)
            return;
        
        // Building Interact
        // This could be IsPlayerOnGround(), why not ?
        if (fetchFloor()) // There will always be a floor under a building <- what do you mean ? "Floor" as in "Tiles", or "Floor" as in "the Floor object" ?
        {
            CheckInteraction();
        }
    }

    private void Item_Explosive(InputAction.CallbackContext context)
    {
        //int current_pixel_ID = gameManager.PositionToPixelID(transform.position);
        //gameManager.CreateQuadAtPixelID(current_pixel_ID);

        int pixel_ID = gameManager.PositionToPixelID(transform.position);
        Debug.Log(pixel_ID);
        Vector3 rayDirection = new Vector3(0, 0, 10);
        int layerMask = LayerMask.GetMask("tile");
        //*
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int ID_to_dig = gameManager.GetPixelAtOffset(pixel_ID, x, y);
                gameManager.CreateQuadAtPixelID(ID_to_dig);

                //*
                Vector3 center_to_dig = gameManager.PixelIDToPosition(ID_to_dig);

                RaycastHit2D hit = Physics2D.Raycast(center_to_dig, rayDirection, rayDirection.magnitude, layerMask);

                if (hit.collider != null)
                {
                    TileScript tile = hit.collider.GetComponent<TileScript>();
                    tile.DigTile();
                }//
            }
        }//*/


        // Probably not the best place to have this
        // Check how many explosives we have
        if (itemExplosiveCount <= 0)
        {
            Debug.Log("No more explosive item.");

            itemExplosiveCount = 0; // Not a necessary check, but inexpensive to do.
            return;
        }

        // Do explosive action
        Debug.Log("Explosive consumed");

        // From player's tileID, get a range of X tiles,
        // loop over that range around the player



        itemExplosiveCount--;
    }

    void Update()
    {
        
    }
    

    private void HandleFallDamage()
    {
        // This feels super janky, but it kinda works
        float threshold = 5;
        float damageMultiplier = 10;


        float currentFrameSpeed = rb.velocity.magnitude;
        if (!omnidirectionalDamage)
        {
            // Clamp negative velocity (going down), then inverse
            currentFrameSpeed = rb.velocity.y;
            currentFrameSpeed = Mathf.Min(currentFrameSpeed, 0);
            currentFrameSpeed *= -1;
        }


        float delta = currentFrameSpeed - previousFrameSpeed;
        if (delta < -threshold)
        {
            float fallDamage = (Mathf.Abs(delta) - threshold) * damageMultiplier;

            TakeDamage((int)fallDamage);
            //Debug.Log("Youch ! Took " + (int)fallDamage + " damage.");
        }

        previousFrameSpeed = currentFrameSpeed;
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

    private void HandleInputs()
    {
        Vector2 inputVector = controls.Gameplay.Movement.ReadValue<Vector2>();

        // Need to disable movement when digging or when the store is open
        // digging is done, but not the store
        if (gameManager.isUpgradeStoreOpen || gameManager.gamePaused)
            return;

        
        // Dig Down
        float down = controls.Gameplay.Down.ReadValue<float>();
        if (down > 0.5)
        {
            if (canPlayerDig())
            {
                Dig(Vector3.down);

                // If the floor is disabled, but the player hasn't gone through it,
                // it would be nice to re-enable it if they use any other direction key.
                // Right now, the player can have two separate holes,
                // disable the floor on one hole (without going through it),
                // then move to the other hole and fall through it, which is weird.
                RaycastHit2D floorHit = fetchFloor();
                if (floorHit)
                {
                    Transform floor = floorHit.transform;
                    floor.GetComponent<Floor>().Disable();
                }
            }
        }

        //Debug.Log(inputVector.x);
        if (inputVector.x < -0.5) // left
        {
            if (canPlayerDig())
                Dig(Vector3.left);
        }
        else if (inputVector.x > 0.5) // right
        {
            if (canPlayerDig())
                Dig(Vector3.right);
        }


        // Apply Movement Force
        if (inputVector.magnitude > 0 && !isDigging)
        {
            Vector2 speedMult = new Vector2(lateralSpeed, upForce) * propellerMultiplier; // could be defined only once
            rb.AddForce(new Vector3(inputVector.x, inputVector.y, 0) * speedMult * Time.deltaTime);


            float lateralMovementCost = 0.5f;
            if (isFalling) // Barely consume fuel when moving left/right while flying
                lateralMovementCost = 0.1f;

            // Flying is more expensive than moving left/right
            float fuelConsumption = inputVector.y + Mathf.Abs(inputVector.x) * lateralMovementCost;
            // Could modulate based on movement input
            // At one point, I want to get movement to be quick from idle, then taper off
            // fuel consumption could reflect that (granted, it's not the most important detail)
            //MovingFuelConsumption(fuelConsumption);
            ReduceFuel(fuelConsumption * Time.deltaTime);
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
                if (disableDigAnimation)
                    tile.DigTile();
                else
                    StartCoroutine("DigAnimation", tile);
            }
        }
    }

    private IEnumerator DigAnimation(TileScript tile)
    {
        //Debug.Log("Diggy diggy hole");
        isDigging = true;

        // We'll have to make sure to not dig if the player just took damage from falling,
        // we don't want to cancel the bounce nor be able to dig at that moment.
        rb.velocity = Vector2.zero;
        rb.simulated = false; // Disable RigidBody when digging

        float digTime = tile.tileInfo.GetDigTime();
        digTime /= drillSpeed;

        Vector3 currentPos = transform.position;
        Vector3 targetPosition = currentPos;
        targetPosition.x = tile.transform.position.x;

        bool diggingDown = (tile.transform.position - currentPos).y < -0.5;
        if (diggingDown)
        {
            targetPosition.y -= 1;
        }


        float startingFuel = currentFuel;
        float targetFuel = currentFuel - tile.tileInfo.GetFuelConsumption();

        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / digTime;

            // Update Player Position
            transform.position = Vector3.Lerp(currentPos, targetPosition, t);
            transform.position += AddJitter();

            // Digging Fuel Consumption
            SetFuel(Mathf.Lerp(startingFuel, targetFuel, t));

            yield return new WaitForSeconds(Time.deltaTime);
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
        float jitterIntensity = 0.05f;
        float jitterScale = 15f;

        float time = Time.realtimeSinceStartup;

        Vector3 jitterOffset;
        jitterOffset.x = Mathf.PerlinNoise((time + 12.4f) * jitterScale, 0f) - 0.5f;
        jitterOffset.y = Mathf.PerlinNoise((time + 9.647f) * jitterScale, 0f) - 0.5f;
        jitterOffset.z = 0;

        jitterOffset *= jitterIntensity;

        return jitterOffset;
    }

    public void TakeDamage(int damage)
    {
        if (disableDamage)
            return;

        currentHealth -= damage;
        hullBar.SetValue(currentHealth);

        // hurt opacity maps 0 - 1 opacity to 0% - Y% damage (compared to max health)
        float opacity = damage / (maxHealth * 0.25f);
        opacity = Mathf.Min(opacity, 1.0f); // clamp to 1.0

        hurtOverlay.Hurt(opacity);
    }

    public void SetHealth(int amount)
    {
        if (disableDamage)
            return;

        currentHealth = amount;
        hullBar.SetValue(currentHealth);
        if(amount < 0)
            hurtOverlay.Hurt(1.0f);
    }

    public void ReduceFuel(float consumption)
    {
        if (disableFuel)
            return;

        if (gameManager.gamePaused)
            return;

        if (gameManager.isUpgradeStoreOpen)
            return;

        currentFuel -= consumption;
        fuelBar.SetValue(currentFuel);
    }

    public void SetFuel(float amount)
    {
        if (disableFuel)
            return;

        currentFuel = amount;
        fuelBar.SetValue(currentFuel);
    }

    // Remove the equivalent of 1 fuel every X seconds
    private IEnumerator FuelConsumption(float seconds)
    {
        float interval = 0.1f; // how often we update
        float consumption = interval / seconds;

        while (true)
        {
            yield return new WaitForSeconds(interval);
            ReduceFuel(consumption);
        }
    }

    private void OnApplicationQuit()
    {
        // This might not be 100% needed,
        // but if I used async instead and didn't stop that,
        // then I would have an error thrown at me when closing the game.
        // This won't hurt. I Promise.
        // Note: iOS applications are usually suspended and do not quit.
        StopCoroutine("FuelConsumption");
        StopCoroutine("DigAnimation");
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

    // Should be in a math library
    public float fit_range(float value, float omin, float omax, float nmin, float nmax)
    {
        return (nmax - nmin) * (value - omin) / (omax - omin) + nmin;
    }

    private bool canPlayerDig()
    {
        // We want a ray that's barely larger than the player. 0.475 is ~half the size of the player.
        RaycastHit2D hit = PlayerRaycast(Vector3.down * 0.475f, "tile", false);
        if(!hit)
            hit = fetchFloor();
        bool grounded = hit.collider != null;

        bool slowEnough = rb.velocity.magnitude < 0.5f;

        // Debug feature to be able to dig as fast as I want
        if (disableDigAnimation)
            slowEnough = true;

        return grounded && slowEnough;
    }

    private RaycastHit2D fetchFloor()
    {
        // We want a ray that's barely larger than the player. 0.475 is ~half the size of the player.
        RaycastHit2D hit = PlayerRaycast(Vector3.down * 0.475f, "floor", false);
        return hit;
    }

    private void CheckInteraction() // Interact with Building, for the time being
    {
        RaycastHit2D hit = PlayerRaycast(Vector3.forward, "building", false);

        if (!hit)
            return;
        if (!hit.transform.GetComponent<Interactable>())
            return;
        

        hit.transform.GetComponent<Interactable>().Interact();
        // We could interact with a store with a menu,
        // but have our velocity move us outside of the bounds
        // such that we can't interact with the store anymore.
        // This is a temporary bandaid/reminder to fix that.
        // Should probably disable rigid body on UI open,
        // then enable it back when the UI is closed
        // so it would be ignored when there's no UI.
        rb.velocity = Vector2.zero;
    }

    // May be useful in the future when doing PlayerState
    // Is different from isPlayerOnTile/Floor since those returns the specific hit object
    public bool IsPlayerOnGround()
    {
        if (isDigging) // If we are digging, we are "on the ground"
            return true;

        bool touchingFloor = rb.IsTouchingLayers(LayerMask.GetMask("floor"));
        // We want a ray that's barely larger than the player. 0.475 is ~half the size of the player.
        RaycastHit2D hit = PlayerRaycast(Vector3.down * 0.475f, "tile", false);
        bool touchingTile = hit.collider != null;

        //Debug.Log("Touching floor " + touchingFloor + " Touching tile " + touchingTile);
        return touchingFloor || touchingTile;
    }
}
