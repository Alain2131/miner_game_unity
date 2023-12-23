using UnityEngine;

public class Store_SellOres : Store
{
    private OreInventory oreInventory;
    //public Money moneyScript;

    private void Start()
    {
        oreInventory = OreInventory.Instance;
    }
    
    public override void Interact()
    {
        Money money = gameManager.money;

        int total = oreInventory.GetTotalValue();
        money.Sell(total);

        oreInventory.RemoveAllOres();
    }
}
