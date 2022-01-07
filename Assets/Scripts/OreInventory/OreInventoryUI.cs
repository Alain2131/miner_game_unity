using UnityEngine;

/* 
 * An OreInventory UI object contains an Ore Inventory Items object.
 * The items are populated on start based on OreInventory's items.
 * For each of them, an OreInventorySlot prefab is Instanciated
 * under Ore Inventory Items.
 */
public class OreInventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public OreInventory oreInventory;
    public GameObject oreInventoryUI;
    public GameObject oreInventorySlotPrefab;
    
    private OreInventorySlot[] slots;

    public static OreInventoryUI Instance;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Populate Ore Inventory Items
        // Instanciate all Ores into the OreInventory

        // Make sure there isn't already something under itemsParent
        foreach (Transform child in itemsParent)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Potential issue, we currently do not have a proper way
        // to order the Inventory Items.
        slots = new OreInventorySlot[oreInventory.OresInCargo.Count];
        int count = 0;
        foreach(OreInventory.OreEntry entry in oreInventory.OresInCargo)
        {
            GameObject newSlot = Instantiate(oreInventorySlotPrefab, itemsParent);
            newSlot.name = "OreInventorySlot_" + entry.OreInfo.name;

            OreInventorySlot slot = newSlot.GetComponent<OreInventorySlot>();
            slot.tileInfo = entry.OreInfo;

            slots[count] = slot;
            count++;

            slot.UpdateSlotUI();
        }

        // Make sure the inventory is closed when the game starts
        oreInventoryUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            oreInventoryUI.SetActive(!IsOpen());

            if (oreInventoryUI.activeSelf) // if we opened the Inventory, update it
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
}
