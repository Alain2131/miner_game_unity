using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Upgrade
{
    // this is shared between all upgrade types.
    // It's not great since the terms cannot differ
    // (cargo CAPACITY versus drill SPEED)
    // And I might want to have an additional property
    // for propeller (Max Weight)
    public string name = "Item Name";
    public string description = "Item Description";
    [Tooltip("The amount of money required to buy this item at a store.")]
    public int cost = 100;
    public float value = 1.0f; // capacity/speed
    // icon
}

public enum upgradeTypes
{
    drill,
    cargo,
    fuel,
    hull,
    propeller
}

[CreateAssetMenu(fileName = "upgrades_item", menuName = "Upgrades/Upgrades Item")]
public class Upgrades : ScriptableObject
{
    public upgradeTypes type = upgradeTypes.drill;
    public List<Upgrade> upgrades;

    public bool BuyUpgrade(upgradeTypes type, int level)
    {
        return GameManager.Instance.upgradeManager.Upgrade(type, level);
    }
}
