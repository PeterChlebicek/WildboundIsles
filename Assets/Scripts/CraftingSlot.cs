using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CraftingSlot : MonoBehaviour
{
    [Header("Crafting Recipe")]
    public Crafting Recipe; // Odkaz na ScriptableObject recept
    [SerializeField] private CraftingManager craftingManager; // Odkaz na CraftingManager

    [SerializeField] private GameObject _highlight;
    private TextMeshProUGUI _amountText;
    private Image _icon; // Image na child objektu Icon

    private void Awake()
    {
        // Najde pøesnì Image komponentu na child objektu Icon
        _icon = transform.Find("Icon")?.GetComponent<Image>();

        if (_icon == null)
        {
            Debug.LogError("Child object 'Icon' with Image component not found! Please ensure there is a child named 'Icon' with an Image component.");
        }

        _amountText = transform.Find("Amount")?.GetComponent<TextMeshProUGUI>();
        if (_amountText == null)
        {
            Debug.LogError("Child object 'Amount' with TextMeshProUGUI component not found! Ensure it exists.");
        }
    }
    private void OnValidate()
    {
        if (_icon == null)
        {
            Awake(); // Zajistí, že _icon je inicializováno i pøi zmìnách v editoru
        }

        UpdateUI();
    }

    public void AssignRecipe(Crafting newRecipe)
    {
        Recipe = newRecipe;
        UpdateUI();
    }

    public void ClearSlot()
    {
        Recipe = null;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_icon == null) return; // Pøidána ochrana, pokud _icon není inicializován

        if (Recipe != null && Recipe.resultItem != null)
        {
            _icon.sprite = Recipe.resultItem.Icon; // Nastaví sprite pouze na Icon
            _icon.gameObject.SetActive(true); // Ujistí se, že Icon je viditelný
            if (Recipe.resultAmount > 1)
            {
                _amountText.text = Recipe.resultAmount.ToString();
            }
            else
            {
                _amountText.text = "";
            }
        }
        else
        {
            _icon.sprite = null; // Odstraní sprite z Icon
            _icon.gameObject.SetActive(false); // Skryje Icon
        }
    }

    public void HighlightSlot(bool highlight)
    {
        _highlight.SetActive(highlight);
    }

    // Nová metoda pro kliknutí na slot
    public void OnSlotClicked()
    {
        if (Recipe != null && craftingManager != null)
        {
            bool crafted = craftingManager.TryCraft(Recipe);
            if (crafted)
            {
                Debug.Log($"Crafting successful: {Recipe.resultItem.Name}");
            }
            else
            {
                Debug.Log("Crafting failed.");
            }
        }
        else
        {
            Debug.LogWarning("CraftingManager or Recipe is not set.");
        }
    }
}
