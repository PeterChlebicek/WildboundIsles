using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private Inventory _inventory; // Odkaz na invent��
    [SerializeField] private List<Crafting> _craftingRecipes; // Seznam v�ech recept�

    // Metoda pro kontrolu, zda lze dan� recept vytvo�it
    public bool CanCraft(Crafting recipe)
    {
        return HasRequiredItems(recipe);
    }

    // Metoda pro pokus o vytvo�en� p�edm�tu podle receptu
    public bool TryCraft(Crafting recipe)
    {
        // Zkontroluj, zda hr�� m� dostatek v�ech po�adovan�ch surovin
        if (!HasRequiredItems(recipe))
        {
            Debug.Log("Nedostatek surovin!");
            return false;
        }

        // Odeber po�adovan� suroviny
        RemoveRequiredItems(recipe);

        // P�idej v�sledn� p�edm�t do invent��e
        _inventory.AddItem(recipe.resultItem, recipe.resultAmount, recipe.resultItem.Durability);

        Debug.Log($"Vytvo�il jsi: {recipe.resultItem.Name} x{recipe.resultAmount}");
        return true;
    }

    // Metoda pro kontrolu, zda hr�� m� dostatek po�adovan�ch surovin
    private bool HasRequiredItems(Crafting recipe)
    {
        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            var requiredItem = recipe.requiredItems[i];
            int requiredAmount = recipe.requiredAmounts[i];

            // Z�sk�me po�et dan�ho druhu suroviny v invent��i
            int itemCount = _inventory.GetItemCountByID(requiredItem.ID);

            // Pokud hr�� nem� dostatek t�to konkr�tn� suroviny, crafting nen� mo�n�
            if (itemCount < requiredAmount)
            {
                Debug.Log($"Chyb� spr�vn� surovina nebo mno�stv�: {requiredItem.Name}, po�adov�no: {requiredAmount}, m�: {itemCount}");
                return false;
            }
        }
        // Pokud v�echny po�adovan� suroviny jsou v dostate�n�m mno�stv�, crafting je mo�n�
        return true;
    }

    // Metoda pro odebr�n� po�adovan�ch surovin z invent��e
    private void RemoveRequiredItems(Crafting recipe)
    {
        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            var requiredItem = recipe.requiredItems[i];
            int remaining = recipe.requiredAmounts[i];

            List<Slot> allSlots = new List<Slot>();
            allSlots.AddRange(_inventory._hotbarSlots); // P�edpokl�d�me p��stup k hotbaru
            allSlots.AddRange(_inventory._inventorySlots); // P�edpokl�d�me p��stup k invent��i

            foreach (var slot in allSlots)
            {
                if (slot.Item != null && slot.Item.ID == requiredItem.ID)
                {
                    if (slot.Amount >= remaining)
                    {
                        slot.RemoveItem(remaining);
                        break;
                    }
                    else
                    {
                        remaining -= slot.Amount;
                        slot.RemoveItem(slot.Amount);
                    }
                }
            }
        }
    }
}
