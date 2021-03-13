﻿
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singleton Code
    private static UIManager _instance;

    public static UIManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Attempted to Instantiate multiple InventoryUIManagers in one scene!");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        if (this == _instance) { _instance = null; }
    }
    #endregion

    [SerializeField]
    private GameObject inventorySlots;

    private InventorySlot[] slots; // This can't be serialized without breaking things

    [SerializeField]
    private GameObject inventoryMenu;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject itemPickupText;

    private bool _itemPickupTextShown = false;
    public bool ItemPickupTextShown {
        get { return _itemPickupTextShown; }
        set
        {
            _itemPickupTextShown = value;
            if (itemPickupText == null)
            {
                Debug.LogError("UIManager does not have a reference to ItemPickupText");
            } else
            {
                itemPickupText.SetActive(_itemPickupTextShown);
            }
        }
    }
    
    void Start()
    {
        slots = inventorySlots.GetComponentsInChildren<InventorySlot>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !pauseMenu.activeSelf) // Toggle inventory menu on Tab
        {
            if (inventoryMenu == null)
            {
                inventoryMenu = GameObject.Find("Inventory Menu");
            }
            inventoryMenu.SetActive(!inventoryMenu.activeSelf);
            GameManager.Instance.CursorLocked = !inventoryMenu.activeSelf; // Cursor locked if inventory not visible, unlocked if visible
        }
        if (Input.GetKeyDown(KeyCode.Escape)) // Close inventory menu on Esc
        {
            if (inventoryMenu.activeSelf)
            {
                inventoryMenu.SetActive(false);
            }
        }
    }

    // Fill inventory slots with all the items the player is carrying. Each slot manages their own appearance.
    public void UpdateInventoryUI() {
        if (slots == null)
        {
            slots = inventorySlots.GetComponentsInChildren<InventorySlot>();
        }

        InventoryItem[] items = GameManager.Instance.GetAllItemsAsArray();
        Debug.Log("Updating Inventory UI");
        if (items.Length > slots.Length) // This shouldn't ever happen, but this is here in case it does
        {
            Debug.LogError("Player is carrying " + (items.Length - slots.Length) + " more items than their inventory capacity!");
            Debug.Log("Slots Length: " + slots.Length);
            Debug.Log("Items Length: " + items.Length);
        }

        for (int i = 0; i < slots.Length && i < items.Length; i++) // We fill out the slots with items, stopping when we run out of items or slots
        {
                slots[i].AddItem(items[i]);
        }
        for (int i = items.Length; i < slots.Length; i++) // Make sure all other slots are empty
        {
            if (slots[i] != null)
            {
                slots[i].ClearSlotIterative();
            }
        }
    }
}