using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private Inventory _inventory; // Odkaz na inventáø
    [SerializeField] private List<Crafting> _craftingRecipes; // Seznam všech receptù

    // Metoda pro kontrolu, zda lze daný recept vytvoøit
    public bool CanCraft(Crafting recipe)
    {
        return HasRequiredItems(recipe);
    }

    // Metoda pro pokus o vytvoøení pøedmìtu podle receptu
    public bool TryCraft(Crafting recipe)
    {
        // Zkontroluj, zda hráè má dostatek všech požadovaných surovin
        if (!HasRequiredItems(recipe))
        {
            Debug.Log("Nedostatek surovin!");
            return false;
        }

        // Odeber požadované suroviny
        RemoveRequiredItems(recipe);

        // Pøidej výsledný pøedmìt do inventáøe
        _inventory.AddItem(recipe.resultItem, recipe.resultAmount, recipe.resultItem.Durability);

        Debug.Log($"Vytvoøil jsi: {recipe.resultItem.Name} x{recipe.resultAmount}");
        return true;
    }

    // Metoda pro kontrolu, zda hráè má dostatek požadovaných surovin
    private bool HasRequiredItems(Crafting recipe)
    {
        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            var requiredItem = recipe.requiredItems[i];
            int requiredAmount = recipe.requiredAmounts[i];

            // Získáme poèet daného druhu suroviny v inventáøi
            int itemCount = _inventory.GetItemCountByID(requiredItem.ID);

            // Pokud hráè nemá dostatek této konkrétní suroviny, crafting není možný
            if (itemCount < requiredAmount)
            {
                Debug.Log($"Chybí správná surovina nebo množství: {requiredItem.Name}, požadováno: {requiredAmount}, má: {itemCount}");
                return false;
            }
        }
        // Pokud všechny požadované suroviny jsou v dostateèném množství, crafting je možný
        return true;
    }

    // Metoda pro odebrání požadovaných surovin z inventáøe
    private void RemoveRequiredItems(Crafting recipe)
    {
        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            var requiredItem = recipe.requiredItems[i];
            int remaining = recipe.requiredAmounts[i];

            List<Slot> allSlots = new List<Slot>();
            allSlots.AddRange(_inventory._hotbarSlots); // Pøedpokládáme pøístup k hotbaru
            allSlots.AddRange(_inventory._inventorySlots); // Pøedpokládáme pøístup k inventáøi

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
