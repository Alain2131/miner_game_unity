using UnityEngine;
using UnityEngine.UI;

// upgrade cost and money are handled in UpgradeSlot.cs

public class UpgradesManager : MonoBehaviour
{
    public Upgrades cargoUpgrades;
    public int cargoUpgradeLevel = 0;

    public Upgrades drillUpgrades;
    public int drillUpgradeLevel = 0;

    public Upgrades fuelUpgrades;
    public int fuelUpgradeLevel = 0;

    public Upgrades hullUpgrades;
    public int hullUpgradeLevel = 0;

    public Upgrades propellerUpgrades;
    public int propellerUpgradeLevel = 0;
    
    public SliderBar fuelSlider;
    public SliderBar healthSlider;

    public Text inventoryMaximumWeight;

    private GameManager game_manager;
    private PlayerScript player_script;

    void Start()
    {
        game_manager = GameManager.Instance;
        player_script = game_manager.playerScript;

        // Apply upgrades
        Upgrade(upgradeTypes.cargo, cargoUpgradeLevel);
        Upgrade(upgradeTypes.drill, drillUpgradeLevel);
        Upgrade(upgradeTypes.fuel, fuelUpgradeLevel);
        Upgrade(upgradeTypes.hull, hullUpgradeLevel);
        Upgrade(upgradeTypes.propeller, propellerUpgradeLevel);
    }

    public int GetUpgradeLevel(upgradeTypes type)
    {
        if (type == upgradeTypes.drill)
            return drillUpgradeLevel;

        if (type == upgradeTypes.cargo)
            return cargoUpgradeLevel;

        if (type == upgradeTypes.fuel)
            return fuelUpgradeLevel;

        if (type == upgradeTypes.hull)
            return hullUpgradeLevel;

        if (type == upgradeTypes.propeller)
            return propellerUpgradeLevel;
        
        return -1;
    }

    public void SetUpgradeLevel(upgradeTypes type, int level)
    {
        if (type == upgradeTypes.drill)
            drillUpgradeLevel = level;

        if (type == upgradeTypes.cargo)
            cargoUpgradeLevel = level;

        if (type == upgradeTypes.fuel)
            fuelUpgradeLevel = level;

        if (type == upgradeTypes.hull)
            hullUpgradeLevel = level;

        if (type == upgradeTypes.propeller)
            propellerUpgradeLevel = level;
    }


    public bool Upgrade(upgradeTypes type, int upgrade_level)
    {
        bool success = false;
        if (type == upgradeTypes.cargo)
            success = Upgrade_Cargo(upgrade_level);
        else if (type == upgradeTypes.drill)
            success = Upgrade_Drill(upgrade_level);
        else if (type == upgradeTypes.fuel)
            success = Upgrade_Fuel(upgrade_level);
        else if (type == upgradeTypes.hull)
            success = Upgrade_Hull(upgrade_level);
        else if (type == upgradeTypes.propeller)
            success = Upgrade_Propeller(upgrade_level);
        else
            Debug.LogError("Unrecognised upgrade type " + type);

        return success;
    }

    private bool Upgrade_Cargo(int upgrade_level)
    {
        //Debug.Log("Cargo");

        Upgrade current_upgrade = cargoUpgrades.upgrades[upgrade_level];
        int new_capacity = (int)current_upgrade.value;
        player_script.cargoSize = new_capacity;
        inventoryMaximumWeight.text = "Maximum Weight : " + new_capacity + "KG";

        return true;
    }

    private bool Upgrade_Drill(int upgrade_level)
    {
        //Debug.Log("Drill");

        Upgrade current_upgrade = drillUpgrades.upgrades[upgrade_level];
        float new_speed = current_upgrade.value;
        player_script.drillSpeed = new_speed;

        return true;
    }

    private bool Upgrade_Fuel(int upgrade_level)
    {
        //Debug.Log("Fuel");

        Upgrade current_upgrade = fuelUpgrades.upgrades[upgrade_level];

        int new_capacity = (int)current_upgrade.value;
        player_script.maxFuel = new_capacity;
        fuelSlider.SetMaxValue(new_capacity);
        
        // refuel
        player_script.currentFuel = new_capacity;

        return true;
    }

    private bool Upgrade_Hull(int upgrade_level)
    {
        //Debug.Log("Hull");

        Upgrade current_upgrade = hullUpgrades.upgrades[upgrade_level];

        int new_capacity = (int)current_upgrade.value;
        player_script.maxHealth = new_capacity;
        healthSlider.SetMaxValue(new_capacity);

        // heal up
        player_script.currentHealth = new_capacity;

        return true;
    }

    // Propeller will have to be done differently.
    // Acceleration based on weight, max speed, something something, right now it feels super bad.
    private bool Upgrade_Propeller(int upgrade_level)
    {
        //Debug.Log("Propeller");

        Upgrade current_upgrade = propellerUpgrades.upgrades[upgrade_level];
        float new_speed = current_upgrade.value;

        player_script.propellerMultiplier = new_speed;

        return true;
    }
}
