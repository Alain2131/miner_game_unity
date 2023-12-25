using UnityEngine;

// upgrade cost and money are handled in UpgradeSlot.cs

public class UpgradesManager : MonoBehaviour
{
    public Upgrades cargo_upgrades;
    public int cargo_upgrade_level = 0;

    public Upgrades drill_upgrades;
    public int drill_upgrade_level = 0;

    public Upgrades fuel_upgrades;
    public int fuel_upgrade_level = 0;

    public Upgrades hull_upgrades;
    public int hull_upgrade_level = 0;

    public Upgrades propeller_upgrades;
    public int propeller_upgrade_level = 0;
    
    public SliderBar fuelSlider;
    public SliderBar healthSlider;

    private GameManager gameManager;
    private PlayerScript playerScript;

    void Start()
    {
        gameManager = GameManager.Instance;
        playerScript = gameManager.playerScript;

        // Apply upgrades
        Upgrade(upgradeTypes.cargo, cargo_upgrade_level);
        Upgrade(upgradeTypes.drill, drill_upgrade_level);
        Upgrade(upgradeTypes.fuel, fuel_upgrade_level);
        Upgrade(upgradeTypes.hull, hull_upgrade_level);
        Upgrade(upgradeTypes.propeller, propeller_upgrade_level);
    }

    public int GetUpgradeLevel(upgradeTypes type)
    {
        if (type == upgradeTypes.drill)
            return drill_upgrade_level;

        if (type == upgradeTypes.cargo)
            return cargo_upgrade_level;

        if (type == upgradeTypes.fuel)
            return fuel_upgrade_level;

        if (type == upgradeTypes.hull)
            return hull_upgrade_level;

        if (type == upgradeTypes.propeller)
            return propeller_upgrade_level;
        
        return -1;
    }

    public void SetUpgradeLevel(upgradeTypes type, int level)
    {
        if (type == upgradeTypes.drill)
            drill_upgrade_level = level;

        if (type == upgradeTypes.cargo)
            cargo_upgrade_level = level;

        if (type == upgradeTypes.fuel)
            fuel_upgrade_level = level;

        if (type == upgradeTypes.hull)
            hull_upgrade_level = level;

        if (type == upgradeTypes.propeller)
            propeller_upgrade_level = level;
    }


    public bool Upgrade(upgradeTypes type, int upgradeLevel)
    {
        bool success = false;
        if (type == upgradeTypes.cargo)
            success = Upgrade_Cargo(upgradeLevel);
        else if (type == upgradeTypes.drill)
            success = Upgrade_Drill(upgradeLevel);
        else if (type == upgradeTypes.fuel)
            success = Upgrade_Fuel(upgradeLevel);
        else if (type == upgradeTypes.hull)
            success = Upgrade_Hull(upgradeLevel);
        else if (type == upgradeTypes.propeller)
            success = Upgrade_Propeller(upgradeLevel);
        else
            Debug.LogError("Unrecognised upgrade type " + type);

        return success;
    }

    private bool Upgrade_Cargo(int upgradeLevel)
    {
        //Debug.Log("Cargo");

        Upgrade currentUpgrade = cargo_upgrades.upgrades[upgradeLevel];
        int newCapacity= (int)currentUpgrade.value;
        playerScript.cargoSize = newCapacity;

        return true;
    }

    private bool Upgrade_Drill(int upgradeLevel)
    {
        //Debug.Log("Drill");

        Upgrade currentUpgrade = drill_upgrades.upgrades[upgradeLevel];
        float newSpeed = currentUpgrade.value;
        playerScript.drillSpeed = newSpeed;

        return true;
    }

    private bool Upgrade_Fuel(int upgradeLevel)
    {
        //Debug.Log("Fuel");

        Upgrade currentUpgrade = fuel_upgrades.upgrades[upgradeLevel];

        int newCapacity = (int)currentUpgrade.value;
        playerScript.maxFuel = newCapacity;
        fuelSlider.SetMaxValue(newCapacity);
        
        // refuel
        playerScript.currentFuel = newCapacity;

        return true;
    }

    private bool Upgrade_Hull(int upgradeLevel)
    {
        //Debug.Log("Hull");

        Upgrade currentUpgrade = hull_upgrades.upgrades[upgradeLevel];

        int newCapacity = (int)currentUpgrade.value;
        playerScript.maxHealth = newCapacity;
        healthSlider.SetMaxValue(newCapacity);

        // heal up
        playerScript.currentHealth = newCapacity;

        return true;
    }

    // Propeller will have to be done differently.
    // Acceleration based on weight, max speed, something something, right now it feels super bad.
    private bool Upgrade_Propeller(int upgradeLevel)
    {
        //Debug.Log("Propeller");

        Upgrade currentUpgrade = propeller_upgrades.upgrades[upgradeLevel];
        float newSpeed = currentUpgrade.value;

        playerScript.propellerMultiplier = newSpeed;

        return true;
    }
}
