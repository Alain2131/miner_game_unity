using UnityEngine;

[System.Serializable]
public class Item
{
    public itemTypes type = itemTypes.explosive;
    public string name = "Item Name";
    public string description = "Item Description";
    [Tooltip("The amount of money required to buy this item at a store.")]
    public int cost = 100;
    // icon
}

public enum itemTypes
{
    explosive,
    teleporter
}

[CreateAssetMenu(fileName = "item_type", menuName = "Items/New Item")]
public class Items : ScriptableObject
{
    public Item item;
}

/*
 * Each items need a special behavior
 * Maybe such behavior won't be stored here,
 * but rather when pressing the shortcut to use the item.
 * 
 * 
 */
