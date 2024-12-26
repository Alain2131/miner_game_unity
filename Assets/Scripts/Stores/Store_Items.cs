using UnityEngine;

public class Store_Items : Store
{
    private void Start()
    {
        // Make sure the store is closed when the game starts
        if (gameManager.isItemsStoreOpen)
            gameManager.ToggleItemsStoreUI();

        
        Item[] items_instance = gameManager.playerScript.items.items_instance;
        foreach (Item item in items_instance)
        {
            // Add an item slot to the consummable items Grid Layout
        }

        // Then, how do we bind the item slot button to do the buying and adding count ?
        //items_instance[0].Count++; // adding count can be as simple as this, assuming we know the id
        // how do we know which inventory slot represents which item ? Do we fetch the name ?
        // I think we'll need to have a script on the inventory slots that will simply reference the item in question as a property or something
        // that script can probably handle setting the various text and icon on the item slot
        // or we could just have the order be dictated by the items_instance[] array.
        // That'd be the order in the item store, and clicking on the third button would buy the third item.
        // That might be the simplest solution, without a middle man.
        // But what would that button be bound to ? And, how would it be bound ?
        // I could use the OnClick() property, can we set that by script ? Is this a good idea ?
        // Also, buying ? What will handle the transaction, making sure we've got enough money, and setting the amount ?

        // Instance item slots
        //Transform upgrade_items_layout = transform.GetChild(0);
        //int co = transform.childCount;
        //Debug.Log(transform);
    }

    public override void Interact()
    {
        // Should disable movement inputs while the store is open
        gameManager.ToggleItemsStoreUI();

        // Kinda eery to be so empty compared to the other stores
        // But I would assume it will be the same for the others
        // once they get a UI. Or this gets completely changed.
    }
}
