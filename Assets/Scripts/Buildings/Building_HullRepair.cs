using UnityEngine;

// The logic isn't super clear or readable, but it works.

// This is basically the same as Building_Refuel
public class Building_HullRepair : Interactable
{
    public override void Interact()
    {
        PlayerScript playerScript = gameManager.player.GetComponent<PlayerScript>();

        int balance = Money.Instance.GetMoney();
        if (balance == 0) // If poor, do nothing.
            return;

        int currentHealth = playerScript.currentHealth;
        int maxHealth = playerScript.maxHealth;
        if (currentHealth == maxHealth)
            return;

        int healthDelta = maxHealth - currentHealth; // how much health we need
        int moneyDelta = balance - healthDelta;

        // 1 health == 1 dollar
        if (moneyDelta < 0) // If we don't have enough money
        {
            Money.Instance.Buy(balance); // Buy what we can
            playerScript.SetHealth(currentHealth + balance);
        }
        else // If we have enough
        {
            Money.Instance.Buy(healthDelta); // buy the difference
            playerScript.SetHealth(maxHealth); // fill the health
        }
    }
}
