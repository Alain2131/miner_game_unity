using UnityEngine;

// The logic isn't super clear or readable, but it works.

// This is basically the same as Building_HullRepair
public class Store_Refuel : Store
{
    public override void Interact()
    {
        PlayerScript player_script = gameManager.playerScript;
        Money money = gameManager.money;

        int balance = money.GetMoney();
        if (balance == 0) // If poor, do nothing.
            return;
        
        int current_fuel = Mathf.CeilToInt(player_script.currentFuel);
        int max_fuel = player_script.maxFuel;
        if (current_fuel == max_fuel)
            return;

        int fuel_delta = max_fuel - current_fuel; // how much fuel we need
        int money_delta = balance - fuel_delta;

        // 1 fuel == 1 dollar
        if(money_delta < 0) // If we don't have enough money
        {
            money.Buy(balance); // Buy what we can
            player_script.SetFuel(current_fuel + balance);
        }
        else // If we have enough
        {
            money.Buy(fuel_delta); // buy the difference
            player_script.SetFuel(max_fuel); // fill the tank
        }
    }
}
