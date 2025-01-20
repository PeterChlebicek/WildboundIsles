using UnityEngine;

[CreateAssetMenu(fileName = "New Perk", menuName = "Perks/Perk")]
public class Perk : ScriptableObject
{
    public string perkName;            // N�zev perku
    public string description;         // Popis perku
    public Sprite icon;                // Ikona pro UI
    public PerkRarity rarity;          // Rarita perku
    public PerkEffectType effectType;  // Typ efektu perku

    // Specifick� parametry pro r�zn� efekty
    public float healthBoost;          // Pro zv��en� maxim�ln�ho zdrav�
    public float staminaBoost;         // Pro zv��en� maxim�ln� staminy
    public float healthRegen;          // Pro zv��en� regenerace zdrav�
    public float staminaRegen;         // Pro zv��en� regenerace staminy
    public bool infiniteStamina;       // Pro nekone�nou staminu
    public float damageBoost;          // Pro zv��en� po�kozen�
    public float lowHealthThreshold;   // Pro efekty p�i n�zk�m zdrav� (nap�. vy��� po�kozen�)
    public float movementSpeedBoost;   // Pro zv��en� rychlosti pohybu
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
    HealthBoost,           // Zv��en� zdrav�
    StaminaBoost,          // Zv��en� staminy
    HealthRegen,           // Zv��en� regenerace zdrav�
    StaminaRegen,          // Zv��en� regenerace staminy
    InfiniteStamina,       // Nekone�n� stamina
    DamageBoost,            
    DamageBoostOnLowHealth, // Zv��en� po�kozen� p�i n�zk�m zdrav�
    MovementSpeedBoost,    // Zv��en� rychlosti pohybu
    JumpBoost,
    Lifesteal,
    saturationBoost
}
