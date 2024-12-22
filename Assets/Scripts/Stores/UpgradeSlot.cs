using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    public Text upgradeCost;
    public Text upgradeName;
    public Text upgradeLevel;

    public Button interact;

    public Upgrades upgradeInfo;

    private GameManager game_manager;
    private UpgradesManager upgrades_manager;
    private Money money;

    private upgradeTypes upgrade_type;
    private int upgrades_count;

    private void Awake()
    {
        game_manager = GameManager.Instance;
        upgrades_manager = game_manager.upgradeManager;
        money = game_manager.money;
    }

    private void Start()
    {
        upgrade_type = upgradeInfo.type;
        upgrades_count = upgradeInfo.upgrades.Count - 1;

        UpdateUI();
    }

    public void UpdateUI()
    {
        int current_level = upgrades_manager.GetUpgradeLevel(upgrade_type);
        if (upgrades_count == current_level)
        {
            interact.interactable = false;

            upgradeCost.text = "Sold Out";
            upgradeName.text = upgradeInfo.upgrades[current_level].name;
            upgradeLevel.text = $"{current_level}/{current_level}";
            
            return;
        }

        upgradeCost.text = upgradeInfo.upgrades[current_level + 1].cost.ToString();
        upgradeName.text = upgradeInfo.upgrades[current_level + 1].name;
        upgradeLevel.text = $"{current_level}/{upgrades_count}";
    }

    public void BuyUpgrade()
    {
        int current_level = upgrades_manager.GetUpgradeLevel(upgrade_type);
        if (upgrades_count == current_level)
        {
            Debug.LogError($"No more upgrades to buy for {upgrade_type}");
            return;
        }

        int balance = money.GetMoney();
        int cost = upgradeInfo.upgrades[current_level + 1].cost;
        if(cost > balance)
        {
            Debug.LogError($"Not enough money to buy {cost}$ {upgrade_type} upgrade.");
            return;
        }

        bool success = game_manager.upgradeManager.Upgrade(upgrade_type, current_level + 1);
        if(success)
        {
            money.Buy(cost);
            upgrades_manager.SetUpgradeLevel(upgrade_type, current_level + 1);
            UpdateUI();
        }
    }
}
