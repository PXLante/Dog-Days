﻿
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        DontDestroyOnLoad(gameObject);
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

    [SerializeField]

    private GameObject doorOpenText;

    [SerializeField]
    private Image dialoguePanel;

    [SerializeField]
    private List<DialogueButtonHandler> dialogueButtons;

    [SerializeField]
    private Text dialogueText;

    [SerializeField]
    private GameObject dialogueInitiationText; // Prompt for starting a dialogue interaction

    private bool _dialogueInitiationTextShown = false;
    public bool DialogueInitiationTextShown
    {
        get { return _dialogueInitiationTextShown; }
        set
        {
            _dialogueInitiationTextShown = value;
            if (dialogueInitiationText == null)
            {
                Debug.LogError("UIManager does not have a reference to DialogueInitiationText");
            }
            else
            {
                dialogueInitiationText.SetActive(_dialogueInitiationTextShown);
            }
        }
    }

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

    private bool _dialoguePanelShown = false;
    public bool DialoguePanelShown
    {
        get { return _dialoguePanelShown; }
        set
        {
            _dialoguePanelShown = value;
            if (dialoguePanel == null)
            {
                dialoguePanel = GameObject.Find("Dialogue Panel").GetComponent<Image>();
            }
            dialoguePanel.enabled = _dialoguePanelShown;

            if (dialogueText == null)
            {
                dialogueText = GameObject.Find("Dialogue Text").GetComponent<Text>();
            }
            dialogueText.enabled = _dialoguePanelShown;
            if (value == false)
            {
                for (int i = 0; i < dialogueButtons.Count; i++)
                {
                    dialogueButtons[i].UpdateButton(false);
                }
            }
        }
    }

    private bool _doorOpenTextShown = false;
    public bool DoorOpenTextShown
    {
        get { return _doorOpenTextShown; }
        set
        {
            _doorOpenTextShown = value;
            if (doorOpenText == null)
            {
                Debug.LogError("Null reference to door opening text");
            } else
            {
                doorOpenText.SetActive(_doorOpenTextShown);
            }
        }
    }

    void Start()
    {
        slots = inventorySlots.GetComponentsInChildren<InventorySlot>();
        UpdateInventoryUI();
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
        //Debug.Log("Updating Inventory UI");
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

    public void DisplayDialogue(DialogueEntry entry)
    {
        // We can pass in a null entry to close the dialogue window
        if (entry == null)
        {
            DialoguePanelShown = false;
            for (int i = 0; i < dialogueButtons.Count; i++)
            {
                dialogueButtons[i].UpdateButton(false); // The button clears itself when no option is passed in to UpdateButton
            }
            GameManager.Instance.CursorLocked = true;
            return;
        } else
        {
            DialoguePanelShown = true;
            dialogueText.text = FormatDialogueText(entry.displayText);
            GameManager.Instance.CursorLocked = false;
        }

        if (entry.options != null)
        {
            for (int i = 0; i < dialogueButtons.Count; i++)
            {
                if (i < entry.options.Count)
                {
                    dialogueButtons[i].UpdateButton(true, entry.options[i]); // Associate each button with a dialogue option
                } else
                {
                    dialogueButtons[i].UpdateButton(false); // The button clears itself when no option is passed in to UpdateButton
                }
            }
        } 
        else
        {
            for (int i = 0; i < dialogueButtons.Count; i++)
            {
                dialogueButtons[i].UpdateButton(false); // The button clears itself when no option is passed in to UpdateButton
            }
        }
    }

    private string FormatDialogueText(string dialogue)
    {
        // Some special text replacement options
        string stringToReturn = dialogue
            .Replace("${currentDog}", GameManager.Instance.currentDog.ToString());

        return stringToReturn;
    }
}