using UnityEngine;

// The logic isn't super clear or readable, but it works.

// This is basically the same as Building_HullRepair
public class Store_Refuel : Store
{
    public override void Interact()
    {
        PlayerScript playerScript = gameManager.playerScript;
        Money money = gameManager.money;

        int balance = money.GetMoney();
        if (balance == 0) // If poor, do nothing.
            return;
        
        int currentFuel = Mathf.CeilToInt(playerScript.currentFuel);
        int maxFuel = playerScript.maxFuel;
        if (currentFuel == maxFuel)
            return;

        int fuelDelta = maxFuel - currentFuel; // how much fuel we need
        int moneyDelta = balance - fuelDelta;

        // 1 fuel == 1 dollar
        if(moneyDelta < 0) // If we don't have enough money
        {
            money.Buy(balance); // Buy what we can
            playerScript.SetFuel(currentFuel + balance);
        }
        else // If we have enough
        {
            money.Buy(fuelDelta); // buy the difference
            playerScript.SetFuel(maxFuel); // fill the tank
        }
    }
}
