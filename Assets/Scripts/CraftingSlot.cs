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
        // Najde p�esn� Image komponentu na child objektu Icon
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
            Awake(); // Zajist�, �e _icon je inicializov�no i p�i zm�n�ch v editoru
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
        if (_icon == null) return; // P�id�na ochrana, pokud _icon nen� inicializov�n

        if (Recipe != null && Recipe.resultItem != null)
        {
            _icon.sprite = Recipe.resultItem.Icon; // Nastav� sprite pouze na Icon
            _icon.gameObject.SetActive(true); // Ujist� se, �e Icon je viditeln�
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
            _icon.sprite = null; // Odstran� sprite z Icon
            _icon.gameObject.SetActive(false); // Skryje Icon
        }
    }

    public void HighlightSlot(bool highlight)
    {
        _highlight.SetActive(highlight);
    }

    // Nov� metoda pro kliknut� na slot
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
