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

    private GameManager game_manager;

    private bool is_digging = false;
    private bool is_falling = false; // flying as well

    private float previous_frame_speed = 0;


    private Controls controls;
    private void Awake()
    {
        game_manager = GameManager.Instance;

        // Bind controls Callbacks
        // rebind -> https://youtu.be/Yjee_e4fICc?t=2478
        // save rebind -> https://youtu.be/Yjee_e4fICc?t=2573
        // mobile (on-screen) controls -> https://youtu.be/Yjee_e4fICc?t=2578

        controls = game_manager.controls;
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

        is_falling = !IsPlayerOnGround();

        HandleFallDamage();
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        game_manager.oreInventory.ToggleInventoryUI();
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        game_manager.TogglePauseGame();
    }

    private void Interact(InputAction.CallbackContext context)
    {
        // Allow for interaction only when the game is not paused
        // Much like gameManager.TogglePauseGame(), this feels like a hack.
        if (game_manager.gamePaused)
            return;
        
        // Building Interact
        // This could be IsPlayerOnGround(), why not ?
        if (FetchFloor()) // There will always be a floor under a building <- what do you mean ? "Floor" as in "Tiles", or "Floor" as in "the Floor object" ?
        {
            CheckInteraction();
        }
    }

    private void Item_Explosive(InputAction.CallbackContext context)
    {
        //int current_pixel_ID = gameManager.PositionToPixelID(transform.position);
        //gameManager.CreateQuadAtPixelID(current_pixel_ID);

        int pixel_ID = game_manager.PositionToPixelID(transform.position);
        Debug.Log(pixel_ID);
        Vector3 ray_direction = new Vector3(0, 0, 10);
        int layer_mask = LayerMask.GetMask("tile");
        //*
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int ID_to_dig = game_manager.GetPixelAtOffset(pixel_ID, x, y);
                game_manager.CreateQuadAtPixelID(ID_to_dig);

                //*
                Vector3 center_to_dig = game_manager.PixelIDToPosition(ID_to_dig);

                RaycastHit2D hit = Physics2D.Raycast(center_to_dig, ray_direction, ray_direction.magnitude, layer_mask);

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
        float damage_multiplier = 10;


        float current_frame_speed = rb.velocity.magnitude;
        if (!omnidirectionalDamage)
        {
            // Clamp negative velocity (going down), then inverse
            current_frame_speed = rb.velocity.y;
            current_frame_speed = Mathf.Min(current_frame_speed, 0);
            current_frame_speed *= -1;
        }


        float delta = current_frame_speed - previous_frame_speed;
        if (delta < -threshold)
        {
            float fall_damage = (Mathf.Abs(delta) - threshold) * damage_multiplier;

            TakeDamage((int)fall_damage);
            //Debug.Log("Youch ! Took " + (int)fallDamage + " damage.");
        }

        previous_frame_speed = current_frame_speed;
    }

    private void ClampPlayerSpeed()
    {
        // Make sure the player can't move faster than a certain speed
        // Especially useful when falling
        float max_speed = 20f;
        float player_speed = rb.velocity.magnitude;
        if (player_speed > max_speed)
        {
            float mult = max_speed / player_speed;
            rb.velocity = new Vector2(rb.velocity.x * mult, rb.velocity.y * mult);
        }
    }

    private void HandleInputs()
    {
        Vector2 input_vector = controls.Gameplay.Movement.ReadValue<Vector2>();

        // Need to disable movement when digging or when the store is open
        // digging is done, but not the store
        if (game_manager.isUpgradeStoreOpen || game_manager.gamePaused)
            return;

        
        // Dig Down
        float down = controls.Gameplay.Down.ReadValue<float>();
        if (down > 0.5)
        {
            if (CanPlayerDig())
            {
                Dig(Vector3.down);

                // If the floor is disabled, but the player hasn't gone through it,
                // it would be nice to re-enable it if they use any other direction key.
                // Right now, the player can have two separate holes,
                // disable the floor on one hole (without going through it),
                // then move to the other hole and fall through it, which is weird.
                RaycastHit2D floor_hit = FetchFloor();
                if (floor_hit)
                {
                    Transform floor = floor_hit.transform;
                    floor.GetComponent<Floor>().Disable();
                }
            }
        }

        //Debug.Log(inputVector.x);
        if (input_vector.x < -0.5) // left
        {
            if (CanPlayerDig())
                Dig(Vector3.left);
        }
        else if (input_vector.x > 0.5) // right
        {
            if (CanPlayerDig())
                Dig(Vector3.right);
        }


        // Apply Movement Force
        if (input_vector.magnitude > 0 && !is_digging)
        {
            Vector2 speedMult = new Vector2(lateralSpeed, upForce) * propellerMultiplier; // could be defined only once
            rb.AddForce(new Vector3(input_vector.x, input_vector.y, 0) * speedMult * Time.deltaTime);


            float lateralMovementCost = 0.5f;
            if (is_falling) // Barely consume fuel when moving left/right while flying
                lateralMovementCost = 0.1f;

            // Flying is more expensive than moving left/right
            float fuelConsumption = input_vector.y + Mathf.Abs(input_vector.x) * lateralMovementCost;
            // Could modulate based on movement input
            // At one point, I want to get movement to be quick from idle, then taper off
            // fuel consumption could reflect that (granted, it's not the most important detail)
            //MovingFuelConsumption(fuelConsumption);
            ReduceFuel(fuelConsumption * Time.deltaTime);
        }
    }

    // "Dig" isn't really what this does, this is just in-between the input and the actual dig
    private void Dig(Vector3 dig_direction)
    {
        // Fetch Tile
        RaycastHit2D hit = PlayerRaycast(dig_direction * 0.6f, "tile", false);

        if (hit.collider != null)
        {
            if (!is_digging)
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
        is_digging = true;

        // We'll have to make sure to not dig if the player just took damage from falling,
        // we don't want to cancel the bounce nor be able to dig at that moment.
        rb.velocity = Vector2.zero;
        rb.simulated = false; // Disable RigidBody when digging

        float dig_time = tile.tileInfo.GetDigTime();
        dig_time /= drillSpeed;

        Vector3 current_pos = transform.position;
        Vector3 target_position = current_pos;
        target_position.x = tile.transform.position.x;

        bool digging_down = (tile.transform.position - current_pos).y < -0.5;
        if (digging_down)
        {
            target_position.y -= 1;
        }


        float starting_fuel = currentFuel;
        float target_fuel = currentFuel - tile.tileInfo.GetFuelConsumption();

        float time = 0f;
        while (time < 1)
        {
            time += Time.deltaTime / dig_time;

            // Update Player Position
            transform.position = Vector3.Lerp(current_pos, target_position, time);
            transform.position += AddJitter();

            // Digging Fuel Consumption
            SetFuel(Mathf.Lerp(starting_fuel, target_fuel, time));

            yield return new WaitForSeconds(Time.deltaTime);
        }

        tile.DigTile();

        rb.simulated = true;
        is_digging = false;
    }

    private Vector3 AddJitter()
    {
        // The Jitter starts and stops abruptly
        // It would be nice to have an easein and easeout
        // over like 0.1 seconds

        // You can make those public at the top to tweak them
        float jitter_intensity = 0.05f;
        float jitter_scale = 15f;

        float time = Time.realtimeSinceStartup;

        Vector3 jitter_offset;
        jitter_offset.x = Mathf.PerlinNoise((time + 12.4f) * jitter_scale, 0f) - 0.5f;
        jitter_offset.y = Mathf.PerlinNoise((time + 9.647f) * jitter_scale, 0f) - 0.5f;
        jitter_offset.z = 0;

        jitter_offset *= jitter_intensity;

        return jitter_offset;
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

        if (game_manager.gamePaused)
            return;

        if (game_manager.isUpgradeStoreOpen)
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
    private RaycastHit2D PlayerRaycast(Vector3 ray_direction, string layer_name = "tile", bool debug_draw = false)
    {
        int layer_mask = LayerMask.GetMask(layer_name);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, ray_direction, ray_direction.magnitude, layer_mask);

        if (debug_draw)
        {
            if (hit.collider != null)
                Debug.DrawRay(transform.position, ray_direction.normalized * hit.distance, Color.green);
            else
                Debug.DrawRay(transform.position, ray_direction, Color.red);
        }

        return hit;
    }

    // Should be in a math library
    public float fit_range(float value, float omin, float omax, float nmin, float nmax)
    {
        return (nmax - nmin) * (value - omin) / (omax - omin) + nmin;
    }

    private bool CanPlayerDig()
    {
        // We want a ray that's barely larger than the player. 0.475 is ~half the size of the player.
        RaycastHit2D hit = PlayerRaycast(Vector3.down * 0.475f, "tile", false);
        if(!hit)
            hit = FetchFloor();
        bool is_player_grounded = hit.collider != null;

        bool slow_enough_to_dig = rb.velocity.magnitude < 0.5f;

        // Debug feature to be able to dig as fast as I want
        if (disableDigAnimation)
            slow_enough_to_dig = true;

        return is_player_grounded && slow_enough_to_dig;
    }

    private RaycastHit2D FetchFloor()
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
        if (is_digging) // If we are digging, we are "on the ground"
            return true;

        bool is_player_touching_floor = rb.IsTouchingLayers(LayerMask.GetMask("floor"));
        // We want a ray that's barely larger than the player. 0.475 is ~half the size of the player.
        RaycastHit2D hit = PlayerRaycast(Vector3.down * 0.475f, "tile", false);
        bool is_player_touching_tile = hit.collider != null;

        //Debug.Log("Touching floor " + touchingFloor + " Touching tile " + touchingTile);
        return is_player_touching_floor || is_player_touching_tile;
    }
}
