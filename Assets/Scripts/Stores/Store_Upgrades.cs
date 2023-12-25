using UnityEngine;

public class Store_Upgrades : Store
{
    public GameObject store_UI;

    private void Start()
    {
        // Make sure the store is closed when the game starts
        store_UI.SetActive(false);
    }

    public override void Interact()
    {
        // Should disable movement inputs while the store is open
        store_UI.SetActive(!store_UI.activeSelf);

        // Kinda eery to be so empty compared to the other stores
        // But I would assume it will be the same for the others
        // once they get a UI. Or this gets completely changed.
    }
}
