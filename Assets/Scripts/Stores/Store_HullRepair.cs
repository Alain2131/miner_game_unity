using UnityEngine;

// The logic isn't super clear or readable, but it works.

// This is basically the same as Building_Refuel
public class Store_HullRepair : Store
{
    public override void Interact()
    {
        PlayerScript player_script = gameManager.playerScript;
        Money money = gameManager.money;

        int balance = money.GetMoney();
        if (balance == 0) // If poor, do nothing.
            return;

        int current_health = player_script.currentHealth;
        int max_health = player_script.maxHealth;
        if (current_health == max_health)
            return;

        int health_delta = max_health - current_health; // how much health we need
        int money_delta = balance - health_delta;

        // 1 health == 1 dollar
        if (money_delta < 0) // If we don't have enough money
        {
            money.Buy(balance); // Buy what we can
            player_script.SetHealth(current_health + balance);
        }
        else // If we have enough
        {
            money.Buy(health_delta); // buy the difference
            player_script.SetHealth(max_health); // fill the health
        }
    }
}
