﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Container class for an item stored in the player's inventory.
public class InventoryItem
{
    public Item item;
    public int amount;

    public InventoryItem(Item item, int amount)
    {
        this.item = item;
        if (this.item == null) // Default to 0 ID item
        {
            this.item = Reference.Instance.GetItemByID(0);
        }
        this.amount = amount;
    }
}