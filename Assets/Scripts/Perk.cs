using UnityEngine;

[CreateAssetMenu(fileName = "New Perk", menuName = "Perks/Perk")]
public class Perk : ScriptableObject
{
    public string perkName;            // Název perku
    public string description;         // Popis perku
    public Sprite icon;                // Ikona pro UI
    public PerkRarity rarity;          // Rarita perku
    public PerkEffectType effectType;  // Typ efektu perku

    // Specifické parametry pro rùzné efekty
    public float healthBoost;          // Pro zvýšení maximálního zdraví
    public float staminaBoost;         // Pro zvýšení maximální staminy
    public float healthRegen;          // Pro zvýšení regenerace zdraví
    public float staminaRegen;         // Pro zvýšení regenerace staminy
    public bool infiniteStamina;       // Pro nekoneènou staminu
    public float damageBoost;          // Pro zvýšení poškození
    public float lowHealthThreshold;   // Pro efekty pøi nízkém zdraví (napø. vyšší poškození)
    public float movementSpeedBoost;   // Pro zvýšení rychlosti pohybu
    public float jumpBoost;
    public float lifesteal;
    public float saturationBoost;
}

public enum PerkRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary,
    Mythical
}

public enum PerkEffectType
{
    HealthBoost,           // Zvýšení zdraví
    StaminaBoost,          // Zvýšení staminy
    HealthRegen,           // Zvýšení regenerace zdraví
    StaminaRegen,          // Zvýšení regenerace staminy
    InfiniteStamina,       // Nekoneèná stamina
    DamageBoost,            
    DamageBoostOnLowHealth, // Zvýšení poškození pøi nízkém zdraví
    MovementSpeedBoost,    // Zvýšení rychlosti pohybu
    JumpBoost,
    Lifesteal,
    saturationBoost
}
