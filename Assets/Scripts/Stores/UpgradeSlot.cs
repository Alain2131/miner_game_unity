using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    public Text upgradeCost;
    public Text upgradeName;
    public Text upgradeLevel;

    public Button interact;

    public Upgrades upgradeInfo;

    private GameManager gameManager;
    private UpgradesManager upgradesManager;
    private Money money;

    private upgradeTypes upgradeType;
    private int upgradesCount;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        upgradesManager = gameManager.upgradeManager;
        money = gameManager.money;
    }

    private void Start()
    {
        upgradeType = upgradeInfo.type;
        upgradesCount = upgradeInfo.upgrades.Count - 1;

        UpdateUI();
    }

    public void UpdateUI()
    {
        int currentLevel = upgradesManager.GetUpgradeLevel(upgradeType);
        if (upgradesCount == currentLevel)
        {
            interact.interactable = false;

            upgradeCost.text = "Sold Out";
            upgradeName.text = upgradeInfo.upgrades[currentLevel].name;
            upgradeLevel.text = currentLevel + "/" + currentLevel;
            
            return;
        }

        upgradeCost.text = upgradeInfo.upgrades[currentLevel + 1].cost.ToString();
        upgradeName.text = upgradeInfo.upgrades[currentLevel + 1].name;
        upgradeLevel.text = currentLevel + "/" + (upgradesCount);
    }

    public void BuyUpgrade()
    {
        int currentLevel = upgradesManager.GetUpgradeLevel(upgradeType);
        if (upgradesCount == currentLevel)
        {
            Debug.LogError("No more upgrades to buy for " + upgradeType);
            return;
        }

        int balance = money.GetMoney();
        int cost = upgradeInfo.upgrades[currentLevel+1].cost;
        if(cost > balance)
        {
            Debug.LogError("Not enough money to buy upgrade " + upgradeType + " at cost " + cost);
            return;
        }

        bool success = upgradeInfo.BuyUpgrade(upgradeType, currentLevel+1);
        if(success)
        {
            money.Buy(cost);
            upgradesManager.SetUpgradeLevel(upgradeType, currentLevel + 1);
            UpdateUI();
        }
    }
}
