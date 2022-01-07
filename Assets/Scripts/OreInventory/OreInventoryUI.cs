using UnityEngine;

public class OreInventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public OreInventory oreInventory;
    public GameObject oreInventoryUI;
    public GameObject oreInventorySlotPrefab;
    
    private OreInventorySlot[] slots;

    void Start()
    {
        // We could automatically populate the slots,
        // but I prefer adding them manually in the Level.
        slots = itemsParent.GetComponentsInChildren<OreInventorySlot>();

        OreInventorySlotsSanityCheck();

        // Make sure the inventory is closed when the game starts
        oreInventoryUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            oreInventoryUI.SetActive(!IsOpen());

            if (oreInventoryUI.activeSelf) // Update the Inventory UI when we open it
                UpdateUI();
        }
    }

    public void UpdateUI()
    {
        foreach(OreInventorySlot slot in slots)
        {
            slot.UpdateSlotUI();
        }
    }

    public bool IsOpen()
    {
        return oreInventoryUI.activeSelf;
    }

    private void OreInventorySlotsSanityCheck()
    {
        // Make sure all ores have a slot
        // This is more for debugging than anything
        bool found;
        foreach (OreInventory.OreEntry entry in OreInventory.Instance.oresInCargo)
        {
            found = false;
            foreach (OreInventorySlot slot in slots)
            {
                if (entry.tileInfo == slot.tileInfo)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Debug.LogError("Missing UI Inventory Slot for " + entry.tileInfo.name);
            }
        }
    }
}
