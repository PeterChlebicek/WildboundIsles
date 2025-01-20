using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting Recipe")]
public class Crafting : ScriptableObject
{
    public string RecipeName; // N�zev receptu
    public InventoryItemData resultItem;
    public InventoryItemData[] requiredItems;
    public int resultAmount;
    public int[] requiredAmounts;
}

