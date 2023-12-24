using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    [SerializeField] private int money = 0;
    [SerializeField] private Text moneyText;

    private void Start()
    {
        updateMoneyUI();
    }

    public int GetMoney()
    {
        return money;
    }

    public void Sell(int revenue)
    {
        money += revenue;
        updateMoneyUI();
    }

    public void Buy(int cost)
    {
        money -= cost;
        updateMoneyUI();
    }

    private void updateMoneyUI()
    {
        moneyText.text = "Moneys : " + money.ToString() + "$";
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
