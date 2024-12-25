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
    public Items items;

    // Might be worth adding pixel_ID as a property here, calculated every frame
    // Other scripts would sample the value instead of calculating it themselves

    private GameManager game_manager;

    private bool is_digging = false;
    private bool is_grounded = true;

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

        // Items
        controls.Gameplay.Explosive.performed += Item_Explosive;
        controls.Gameplay.LargeExplosive.performed += Item_LargeExplosive;
        controls.Gameplay.CheapTeleporter.performed += Item_CheapTeleporter;
        controls.Gameplay.FancyTeleporter.performed += Item_FancyTeleporter;
        controls.Gameplay.RepairKit.performed += Item_RepairKit;
        controls.Gameplay.FuelReserve.performed += Item_FuelReserve;

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

        is_grounded = IsPlayerOnGround();

        HandleFallDamage();

        MoveAwayFromWall();
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
        items.explosive.ConsumeItem();
    }

    private void Item_LargeExplosive(InputAction.CallbackContext context)
    {
        items.large_explosive.ConsumeItem();
    }

    private void Item_CheapTeleporter(InputAction.CallbackContext context)
    {
        items.cheap_teleporter.ConsumeItem();
    }

    private void Item_FancyTeleporter(InputAction.CallbackContext context)
    {
        items.fancy_teleporter.ConsumeItem();
    }

    private void Item_RepairKit(InputAction.CallbackContext context)
    {
        items.repair_kit.ConsumeItem();
    }

    private void Item_FuelReserve(InputAction.CallbackContext context)
    {
        items.fuel_reserve.ConsumeItem();
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
            //Debug.Log($"Youch ! Took {(int)fall_damage} damage.");
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

        // Lateral drag
        float flat_reduction = 0.015f; // mostly useful at low speed
        flat_reduction = Mathf.Min(flat_reduction, Mathf.Abs(rb.velocity.x)); // Ensure we don't remove more velocity than we have
        flat_reduction *= Mathf.Sign(rb.velocity.x);

        float mult_reduction = 0.9975f; // mostly useful at high speed
        rb.velocity = new Vector2((rb.velocity.x - flat_reduction) * mult_reduction, rb.velocity.y);
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


        if (input_vector.magnitude > 0 && !is_digging)
        {
            // Apply Movement Force
            Vector2 speed_mult = new Vector2(lateralSpeed, upForce) * propellerMultiplier; // could be defined only once
            rb.AddForce(new Vector3(input_vector.x, input_vector.y, 0) * speed_mult * Time.deltaTime);


            // Apply fuel consumption
            float lateral_movement_cost = 0.1f;
            if (is_grounded) // Higher lateral movement fuel consumption when grounded
                lateral_movement_cost = 0.5f;

            // Flying is more expensive than moving left/right
            float fuel_consumption = input_vector.y + Mathf.Abs(input_vector.x) * lateral_movement_cost;
            // Could modulate based on movement input
            // At one point, I want to get movement to be quick from idle, then taper off
            // fuel consumption could reflect that (granted, it's not the most important detail)
            //MovingFuelConsumption(fuelConsumption);
            ReduceFuel(fuel_consumption * Time.deltaTime);
        }
    }

    // Push player away from walls a little bit so they don't slide right along it.
    // Otherwise, there is a potential to hit a "stray collision" (I believe it's a precision issue).
    private void MoveAwayFromWall()
    {
        if (is_grounded)
            return;

        const float HALF_PLAYER = 0.45f;
        const float BIAS = 0.015f;
        const float DISTANCE_THRESHOLD = HALF_PLAYER + BIAS;

        // Detect walls
        bool[] walls = { false, false };
        Vector2 direction = new Vector2(-1, 0);
        RaycastHit2D hit = PlayerRaycast(direction * DISTANCE_THRESHOLD, "tile", false);
        if (hit)
            walls[0] = true;
        else
        {
            direction.x = 1;
            hit = PlayerRaycast(direction * DISTANCE_THRESHOLD, "tile", false);
            if (hit)
                walls[1] = true;
        }

        // Move away from wall
        Vector3 pos = transform.position;
        if (walls[0]) // left wall
            pos.x += BIAS;
        else if (walls[1]) // right wall
            pos.x -= BIAS;
        else // no wall
            return;

        // Purposefully not a force, we don't want momentum from a wall
        // to push us into the other wall
        transform.position = pos;
    }

    // "Dig" isn't really what this does, this is just in-between the input and the actual dig
    private void Dig(Vector3 dig_direction)
    {
        float ray_length = 0.6f;
        if (disableDigAnimation)
            ray_length = 1.0f; // Allows for unlimited digging speed

        // Fetch Tile
        RaycastHit2D hit = PlayerRaycast(dig_direction * ray_length, "tile", false);

        if (hit.collider == null || is_digging)
            return;

        // quite hacky at the moment
        int down = dig_direction.y < -0.5 ? 1 : 0;
        int right = dig_direction.x > 0.5 ? 1 : 0;
        int left = dig_direction.x < -0.5 ? 1 : 0;
        int lateral = -left + right;
        int player_pixel_ID = game_manager.PositionToPixelID(transform.position);
        int pixel_ID = game_manager.GetPixelIDAtOffset(player_pixel_ID, lateral, down);

        //Debug.Log($"player_ID {player_pixel_ID}, tile ID {pixel_ID}, down {down}, lateral {lateral}");
        
        // Make sure we didn't just bounce
        // This might have to be handled with a PlayerState
        // As long as we haven't been on the ground for X amount of time,
        // we are not allowed to dig (Falling State)
        // if (playerState.onGround) doDig();
        if (disableDigAnimation)
            game_manager.DigTile(pixel_ID);
        else
            StartCoroutine(DigAnimation(pixel_ID));
    }

    private IEnumerator DigAnimation(int pixel_ID)
    {
        TileInfo tile_info = game_manager.PixelIDToTileInfo(pixel_ID);

        //Debug.Log("Diggy diggy hole");
        is_digging = true;

        // We'll have to make sure to not dig if the player just took damage from falling,
        // we don't want to cancel the bounce nor be able to dig at that moment.
        rb.velocity = Vector2.zero;
        rb.simulated = false; // Disable RigidBody when digging

        float dig_time = tile_info.GetDigTime();
        dig_time /= drillSpeed;

        Vector2 current_pos = transform.position;
        Vector2 target_position = game_manager.PixelIDToPosition(pixel_ID);


        float starting_fuel = currentFuel;
        float target_fuel = currentFuel - tile_info.GetFuelConsumption();

        float time = 0f;
        while (time < 1)
        {
            time += Time.deltaTime / dig_time;

            // Update Player Position
            transform.position = Vector2.Lerp(current_pos, target_position, time);
            transform.position += AddJitter();

            // Digging Fuel Consumption
            SetFuel(Mathf.Lerp(starting_fuel, target_fuel, time));

            yield return new WaitForSeconds(Time.deltaTime);
        }

        //tile.DigTile();
        game_manager.DigTile(pixel_ID);

        rb.simulated = true;
        is_digging = false;
    }

    // Has to return vector3 even though we're only doing a 2D jitter,
    // otherwise this doesn't work -> transform.position += AddJitter();
    private Vector3 AddJitter()
    {
        // The Jitter starts and stops abruptly
        // It would be nice to have an easein and easeout
        // over like 0.1 seconds

        // You can make those public at the top to tweak them
        float jitter_intensity = 0.05f;
        float jitter_scale = 15f;

        float time = Time.realtimeSinceStartup;

        Vector2 jitter_offset;
        jitter_offset.x = Mathf.PerlinNoise((time + 12.4f) * jitter_scale, 0f) - 0.5f;
        jitter_offset.y = Mathf.PerlinNoise((time + 9.647f) * jitter_scale, 0f) - 0.5f;

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

        currentHealth = Mathf.Min(amount, maxHealth);
        hullBar.SetValue(currentHealth);

        // Should probably put the "Is Player Now Dead" logic here
        // It's currently in hullBar.setValue()
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

        currentFuel = Mathf.Min(amount, maxFuel);
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
    private RaycastHit2D PlayerRaycast(Vector3 ray_origin, Vector3 ray_direction, string layer_name = "tile", bool debug_draw = false)
    {
        int layer_mask = LayerMask.GetMask(layer_name);
        RaycastHit2D hit = Physics2D.Raycast(ray_origin, ray_direction, ray_direction.magnitude, layer_mask);

        if (debug_draw)
        {
            if (hit.collider != null)
                Debug.DrawRay(ray_origin, ray_direction.normalized * hit.distance, Color.green);
            else
                Debug.DrawRay(ray_origin, ray_direction, Color.red);
        }

        return hit;
    }

    private RaycastHit2D PlayerRaycast(Vector3 ray_direction, string layer_name = "tile", bool debug_draw = false)
    {
        return PlayerRaycast(transform.position, ray_direction, layer_name, debug_draw);
    }

    // Should be in a math library
    public float fit_range(float value, float omin, float omax, float nmin, float nmax)
    {
        return (nmax - nmin) * (value - omin) / (omax - omin) + nmin;
    }

    private bool CanPlayerDig()
    {
        if (disableDigAnimation)
            return true;

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

        if (!hit)
            return hit;

        if(hit.collider.isTrigger) // ignore triggers
            return new RaycastHit2D();
        
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
    // Is susceptible to detect hit-velocity impact on a wall as being grounded, for a frame or two.
    // I'd like a way to filter those out, or make a better grounded detection mechanism.
    public bool IsPlayerOnGround()
    {
        if (is_digging) // If we are digging, we are "on the ground"
            return true;

        //bool is_player_touching_floor = rb.IsTouchingLayers(LayerMask.GetMask("floor")); // doesn't interact well with the new floor trigger
        bool is_player_touching_floor = FetchFloor();

        // We want a ray that's barely larger than the player. Player is a 1x1 square scaled to 0.9, and half of 0.9 is 0.45
        const float PLAYER_HALF_SIZE = 0.45f;
        const float RAY_BIAS = 0.01f;
        Vector3 ray_origin = transform.position + new Vector3(PLAYER_HALF_SIZE - 0.01f, 0.0f, 0.0f);
        RaycastHit2D hit_left = PlayerRaycast(ray_origin, Vector3.down * (PLAYER_HALF_SIZE + RAY_BIAS), "tile", false);
        ray_origin = transform.position - new Vector3(PLAYER_HALF_SIZE - 0.01f, 0.0f, 0.0f);
        RaycastHit2D hit_right = PlayerRaycast(ray_origin, Vector3.down * (PLAYER_HALF_SIZE + RAY_BIAS), "tile", false);

        bool is_player_touching_tile = hit_left.collider != null || hit_right.collider != null;

        //Debug.Log("Touching floor " + touchingFloor + " Touching tile " + touchingTile);
        return is_player_touching_floor || is_player_touching_tile;
    }
}
