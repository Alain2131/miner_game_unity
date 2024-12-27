using UnityEngine;

public class Store_Items : Store
{
    private Item[] items_instance;

    private void Start()
    {
        // Make sure the store is closed when the game starts
        if (gameManager.isItemsStoreOpen)
            gameManager.ToggleItemsStoreUI();

        
        items_instance = gameManager.playerScript.items.items_instance;
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

        /*
        Update, I did the simplest solution for the time being.
        I manually copy-pasted the UI stuff, and linked each buttons to Store_Items.BuyItem(),
        as well as manually setting the proper index, name and cost.
        This WILL have to be done better - but I don't think we need a big system.
        How often will these change ? Just do something basic to set the name and cost by id.

        As far as what handles the buying and stuff - that's prime refactoring real estate !
        */
    }

    public override void Interact()
    {
        // Should disable movement inputs while the store is open
        gameManager.ToggleItemsStoreUI();

        // Kinda eery to be so empty compared to the other stores
        // But I would assume it will be the same for the others
        // once they get a UI. Or this gets completely changed.
    }

    public void BuyItem(int index)
    {
        Item item = items_instance[index];

        bool bought = gameManager.money.Buy(item.Cost);
        if (!bought)
        {
            Debug.LogWarning($"Not enough momeys to buy {item.Name}");
            return;
        }

        item.Count++;
        Debug.Log($"bought {item.Name}, has {item.Count}");
    }
}
