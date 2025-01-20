using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory Item")]
public class InventoryItemData : ScriptableObject
{
    public int ID;
    public string Name;
    public Sprite Icon;
    public GameObject Prefab;
    public int StackSize;
    public int Durability;
}
