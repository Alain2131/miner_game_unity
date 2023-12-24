using UnityEngine;

public class Store_SellOres : Store
{
    private OreInventory oreInventory;

    private void Start()
    {
        oreInventory = gameManager.oreInventory;
    }
    
    public override void Interact()
    {
        Money money = gameManager.money;

        int total = oreInventory.GetTotalValue();
        money.Sell(total);

        oreInventory.RemoveAllOres();
    }
}
