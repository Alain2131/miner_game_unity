using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    [SerializeField] private int money = 0;
    [SerializeField] private Text moneyText;

    private void Start()
    {
        UpdateMoneyUI();
    }

    public int GetMoney()
    {
        return money;
    }

    public void Sell(int revenue)
    {
        money += revenue;
        UpdateMoneyUI();
    }

    public bool Buy(int cost)
    {
        if (money < cost)
            return false;

        money -= cost;
        UpdateMoneyUI();
        return true;
    }

    private void UpdateMoneyUI()
    {
        moneyText.text = $"Moneys : {money}$";
    }

    // Add an object in-game with a green/red color
    // showing how much money was earned/spent
    // Make it move a bit and fade out pretty fast
    // This will require a custom MoneyFlavor Prefab reference
    // which we will instanciate here at the player's location
    // and that prefab will handle itself (animation and death)
    public void MoneyFlavor(int count)
    {
        // Instanciate Prefab at player's location
        // Specify the amount earned/spent
        // The Prefab will handle itself from thereon
    }
}
