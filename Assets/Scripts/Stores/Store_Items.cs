using UnityEngine;

public class Store_Items : Store
{
    private void Start()
    {
        // Make sure the store is closed when the game starts
        if (gameManager.isItemsStoreOpen)
            gameManager.ToggleItemsStoreUI();
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
