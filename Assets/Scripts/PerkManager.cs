using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerkManager : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject perkPanel; // Panel v UI, kam se p�id�vaj� ikony perk�
    public GameObject perkIconPrefab; // Prefab pro ikonu perku (pr�zdn� Image prefab)
    public GameObject tooltip; // Tooltip objekt
    public TextMeshProUGUI tooltipTitle; // TMP Text pro n�zev
    public TextMeshProUGUI tooltipDescription; // TMP Text pro popis

    private List<Perk> activePerks = new List<Perk>(); // Seznam aktivn�ch perk�
    private Player player; // Odkaz na hr��e
    private Tool tool;

    void Start()
    {
        player = GetComponent<Player>(); // Z�sk�n� odkazu na hr��e
        if (tooltip != null) tooltip.SetActive(false); // Skryj tooltip na za��tku
    }

    public void ApplyPerk(Perk perk)
    {
        // Pokud hr�� tento perk je�t� nem�, p�idej ho
        if (!activePerks.Contains(perk))
        {
            activePerks.Add(perk); // P�id�n� do seznamu aktivn�ch perk�
            AddPerkToUI(perk);    // Zobraz perk v UI
            ApplyPerkEffect(perk); // Aplikuj efekt perku
            Debug.Log($"Perk {perk.perkName} applied!");
        }
        else
        {
            Debug.Log($"Player already has perk {perk.perkName}");
        }
    }

    private void AddPerkToUI(Perk perk)
    {
        GameObject newIcon = Instantiate(perkIconPrefab, perkPanel.transform);
        Image iconImage = newIcon.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = perk.icon;
        }
        else
        {
            Debug.LogWarning("PerkIconPrefab does not have an Image component!");
        }

        EventTrigger trigger = newIcon.AddComponent<EventTrigger>();
        AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) => ShowTooltip(perk, Input.mousePosition));
        AddEventTrigger(trigger, EventTriggerType.PointerExit, (data) => HideTooltip());
    }
    private void ShowTooltip(Perk perk, Vector3 position)
    {
        if (tooltip != null)
        {
            // Aktivace Tooltipu
            tooltip.SetActive(true);

            // P�id�n� offsetu pro pozici (nap�. napravo od my�i)
            Vector3 offset = new Vector3(15f, -15f, 0f); // Upravit dle pot�eby
            tooltip.transform.position = position + offset;

            // Aktualizace textu
            tooltipTitle.text = perk.perkName;
            tooltipDescription.text = perk.description;
        }
    }


    private void HideTooltip()
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false);
        }
    }
    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType; // Pou�ijeme eventID m�sto eventType
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private void ApplyPerkEffect(Perk perk)
    {
        if (player == null)
        {
            Debug.LogError("Player component not found!");
            return;
        }

        switch (perk.effectType)
        {
            case PerkEffectType.InfiniteStamina:
                player.hasInfiniteStamina = true;
                break;
            case PerkEffectType.MovementSpeedBoost:
                player.walkSpeed = player.walkSpeed * perk.movementSpeedBoost;
                player.sprintSpeed = player.sprintSpeed * perk.movementSpeedBoost;
                break;
            case PerkEffectType.JumpBoost:
                player.jumpForce = player.jumpForce * perk.jumpBoost;
                break;
            case PerkEffectType.DamageBoost:
                // Aplikov�n� bonusu na po�kozen� n�stroje
                if (tool != null)
                {
                    tool.UpdateToolDamage(perk.damageBoost); // Aplikuj modifik�tor po�kozen�
                }
                break;
            case PerkEffectType.DamageBoostOnLowHealth:
                if (player.currentHealth <= perk.lowHealthThreshold)
                {
                    if (tool != null)
                    {
                        tool.UpdateToolDamage(perk.damageBoost); // Aplikuj modifik�tor po�kozen�
                    }
                }
                break;
            case PerkEffectType.Lifesteal:
                if (tool != null)
                {
                    tool.lifesteal = perk.lifesteal;
                }
                break;
            default:
                Debug.LogWarning($"Effect for perk {perk.perkName} not defined.");
                break;
        }
    }
    private void ApplyLifesteal(float damageDealt, Perk perk)
    {
        float lifestealAmount = damageDealt * perk.lifesteal;

        player.currentHealth += lifestealAmount;

        if (player.currentHealth > player.maxHealth)
        {
            player.currentHealth = player.maxHealth;
        }

        Debug.Log($"Lifesteal applied: {lifestealAmount}, Current Health: {player.currentHealth}");
    }
}