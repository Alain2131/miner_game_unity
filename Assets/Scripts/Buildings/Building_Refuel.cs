using UnityEngine;

// The logic isn't super clear or readable, but it works.

// This is basically the same as Building_HullRepair
public class Building_Refuel : Interactable
{
    public override void Interact()
    {
        PlayerScript playerScript = gameManager.player.GetComponent<PlayerScript>();

        int balance = Money.Instance.GetMoney();
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
            Money.Instance.Buy(balance); // Buy what we can
            playerScript.SetFuel(currentFuel + balance);
        }
        else // If we have enough
        {
            Money.Instance.Buy(fuelDelta); // buy the difference
            playerScript.SetFuel(maxFuel); // fill the tank
        }
    }
}
