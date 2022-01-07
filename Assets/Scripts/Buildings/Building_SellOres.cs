using UnityEngine;

public class Building_SellOres : Interactable
{
    private OreInventory oreInventory;

    private void Start()
    {
        oreInventory = OreInventory.Instance;
    }
    
    public override void Interact()
    {
        int total = oreInventory.GetTotalValue();
        Money.Instance.Sell(total);

        oreInventory.RemoveAll();
    }
}
