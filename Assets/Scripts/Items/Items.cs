using UnityEngine;


/*
Adding a new item :
1. Create new item subclass in Items.cs (that's here !)
Set relevant settings, implement ConsumeItem()
2. Instance new item in the Items class (within Start())
3. Add a Controls entry for the item
Set a proper shortcut for it
4. Bind shortcut to the new item's ConsumeItem() method
In PlayerScript.cs, create a new Item_[ItemName]() method
Bind it to the new item
Then bind the control scheme within Awake()
5. Done !
There might be more steps later on, say for save game or the item store.
*/

public class Item
{
    public virtual string Name { get; }
    public virtual string Description { get; }
    public virtual int Count { get; set; }
    public virtual int Cost { get; }

    // icon

    public virtual void ConsumeItem()
    {
        Debug.Log("Consume Item not implemented");
    }
}

public class Explosive : Item
{
    public override string Name => "Explosive";

    public override string Description => "An explosive item.";

    public override int Cost => 100;

    public override void ConsumeItem()
    {
        if (Count <= 0)
        {
            Debug.LogWarning("No more explosive item.");

            Count = 0; // Not a necessary check, but inexpensive to do.
            return;
        }

        GameManager game_manager = GameManager.Instance;
        Transform player_transform = game_manager.player;

        int pixel_ID = game_manager.PositionToPixelID(player_transform.position);
        const int EXPLOSION_RANGE = 1;

        // Loop over a range of 1 tile around the player (total of 8)
        for (int x = -EXPLOSION_RANGE; x <= EXPLOSION_RANGE; x++)
        {
            for (int y = -EXPLOSION_RANGE; y <= EXPLOSION_RANGE; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int ID_to_dig = game_manager.GetPixelIDAtOffset(pixel_ID, x, y);
                if (ID_to_dig < 0)
                    continue;

                //gameManager.CreateQuadAtPixelID(ID_to_dig);

                game_manager.DigTile(ID_to_dig);
            }
        }

        Count--;
    }
}

// Could probably be a child of Explosive,
// where explosive implements an ExplosionRadius variable
// that can be overrided by LargeExplosive...
// Would have to have a different message for "no more item"
// Would have to have a different EXPLOSION_RANGE
public class LargeExplosive : Item
{
    public override string Name => "Large Explosive";

    public override string Description => "Big kaboom.";

    public override int Cost => 500;

    public override void ConsumeItem()
    {
        if (Count <= 0)
        {
            Debug.LogWarning("No more kaboom-y item.");

            Count = 0; // Not a necessary check, but inexpensive to do.
            return;
        }

        GameManager game_manager = GameManager.Instance;
        Transform player_transform = game_manager.player;

        int pixel_ID = game_manager.PositionToPixelID(player_transform.position);
        const int EXPLOSION_RANGE = 2;

        // Loop over a range of 1 tile around the player (total of 8)
        for (int x = -EXPLOSION_RANGE; x <= EXPLOSION_RANGE; x++)
        {
            for (int y = -EXPLOSION_RANGE; y <= EXPLOSION_RANGE; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int ID_to_dig = game_manager.GetPixelIDAtOffset(pixel_ID, x, y);
                if (ID_to_dig < 0)
                    continue;

                //gameManager.CreateQuadAtPixelID(ID_to_dig);

                game_manager.DigTile(ID_to_dig);
            }
        }

        Count--;
    }
}

public class CheapTeleporter : Item
{
    public override string Name => "Cheap Teleporter";

    public override string Description => "Teleports you over ground. Who knows what damage will ensue.";

    public override int Cost => 500;

    public override void ConsumeItem()
    {
        if (Count <= 0)
        {
            Debug.LogWarning("No more teleportation item.");

            Count = 0;
            return;
        }

        Vector2 TELEPORT_LOCATION = new Vector2(25, 2);

        GameManager game_manager = GameManager.Instance;
        Transform player_transform = game_manager.player;
        PlayerScript player = game_manager.playerScript;

        player_transform.position = TELEPORT_LOCATION;

        // Random velocity
        // Those numbers will have to be balanced when cargo weight will be implemented
        float x_velocity = Random.Range(2, 10);
        float y_velocity = Random.Range(-5, -15);
        player.rb.velocity = new Vector2(x_velocity, y_velocity);

        Count--;
    }
}

// Again, could probably be a child of CheapTeleporter
// with overrides for TELEPORT_LOCATION and velocity
public class FancyTeleporter : Item
{
    public override string Name => "Teleporter";

    public override string Description => "Safely teleports you over ground.";

    public override int Cost => 2000;

    public override void ConsumeItem()
    {
        if (Count <= 0)
        {
            Debug.LogWarning("No more teleportation item.");

            Count = 0;
            return;
        }

        Vector2 TELEPORT_LOCATION = new Vector2(5, 0.5f);

        GameManager game_manager = GameManager.Instance;
        Transform player_transform = game_manager.player;
        PlayerScript player = game_manager.playerScript;

        player_transform.position = TELEPORT_LOCATION;

        // Reset velocity
        // It's supposed to be safe, but the impact detection can detect a large change in velocity and still hurt the player
        // Will have to take that into account
        player.rb.velocity = new Vector2(0, 0);

        Count--;
    }
}

public class RepairKit : Item
{
    public override string Name => "Repair Kit";

    public override string Description => "Repairs your hull a small amount.";

    public override int Cost => 200;

    public override void ConsumeItem()
    {
        if (Count <= 0)
        {
            Debug.LogWarning("No more repair kit item.");

            Count = 0;
            return;
        }

        int REPAIR_AMOUNT = 20;

        GameManager game_manager = GameManager.Instance;
        PlayerScript player = game_manager.playerScript;

        
        player.SetHealth(player.currentHealth + REPAIR_AMOUNT);

        Count--;
    }
}

public class FuelReserve : Item
{
    public override string Name => "Fuel Reserve";

    public override string Description => "A bit of fuel to top you up in a pinch.";

    public override int Cost => 300;

    public override void ConsumeItem()
    {
        if (Count <= 0)
        {
            Debug.LogWarning("No more fuel reserve item.");

            Count = 0;
            return;
        }

        int REFUEL_AMOUNT = 20;

        GameManager game_manager = GameManager.Instance;
        PlayerScript player = game_manager.playerScript;


        player.SetFuel(player.currentFuel + REFUEL_AMOUNT);

        Count--;
    }
}


public class Items : MonoBehaviour
{
    public Item explosive;
    public Item large_explosive;
    public Item cheap_teleporter;
    public Item fancy_teleporter;
    public Item repair_kit;
    public Item fuel_reserve;

    public Item[] items_instance;

    private void Awake()
    {
        explosive = new Explosive();
        large_explosive = new LargeExplosive();
        cheap_teleporter = new CheapTeleporter();
        fancy_teleporter = new FancyTeleporter();
        repair_kit = new RepairKit();
        fuel_reserve = new FuelReserve();

        // array to gather all types of items for other scripts
        // i.e. store, save game can go over all items
        items_instance = new Item[6];
        items_instance[0] = explosive;
        items_instance[1] = large_explosive;
        items_instance[2] = cheap_teleporter;
        items_instance[3] = fancy_teleporter;
        items_instance[4] = repair_kit;
        items_instance[5] = fuel_reserve;

        // These will probably be set by save data
        foreach (Item item in items_instance)
        {
            item.Count = 5;
        }
    }
}
