using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] public string toolName;  // Název nástroje
    [SerializeField] public float damage;      // Poškození nástroje
    [SerializeField] public int tier;        // Tier nástroje  
    [SerializeField] private float interactionRange = 3f; // Maximální vzdálenost interakce
    [SerializeField] private LayerMask resourceLayer; // Vrstva, která obsahuje resource objekty
    [SerializeField] private GameObject hitEffectPrefab; // Prefab particle systému pro efekt zásahu

    // Nové vlastnosti
    [SerializeField] private bool canMineOre;    // Jestli nástroj může těžit rudy
    [SerializeField] private bool canChopTree;  // Jestli nástroj může těžit stromy

    public float lifesteal;

    // Zvuky
    [SerializeField] private AudioClip stoneHit;  // Zvuk při zásahu rudy
    [SerializeField] private AudioClip woodHit;   // Zvuk při zásahu stromu
    [SerializeField] private AudioClip toolSwing; // Zvuk při švihu nástroje
    private AudioSource audioSource;             // AudioSource komponenta

    private Animator _animator;

    private Player player;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; 
        }
    }

    void Update()
    {
        // Spuštění animace a raycastu při levém tlačítku myši
        if (Input.GetMouseButtonDown(0))
        {
            // Spuštění animace
            _animator.Play("ItemAttack");

            // Vykonání raycastu
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionRange, resourceLayer))
            {
                // Spawn particle efektu na místě zásahu
                SpawnHitEffect(hit.point, hit.normal);

                // Interakce s resource objekty
                if (hit.collider.CompareTag("ResourceOre") && canMineOre)
                {
                    Debug.Log("Ore mined!");
                    PlaySound(stoneHit);
                }
                else if (hit.collider.CompareTag("ResourceTree") && canChopTree)
                {
                    Debug.Log("Tree chopped!");
                    PlaySound(woodHit);
                }
                else
                {
                    PlaySound(toolSwing);
                }

                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    // Poškození nepřítele
                    enemy.HurtEnemy(damage);
                    PlaySound(toolSwing);
                    ApplyLifesteal(lifesteal);
                }

                // Pokud je to Resource objekt (např. pro kontrolu tieru)
                Resource resource = hit.collider.GetComponent<Resource>();
                if (resource != null)
                {
                    if (tier >= resource.RequiredTier)
                    {
                        // Těžba suroviny
                        resource.Harvest(resource, damage);
                        _inventory.DurabilityDamage();
                    }
                    else
                    {
                        Debug.Log("Tier of this tool is too low for this resource!");
                    }
                }
            }
            else
            {
                PlaySound(toolSwing);
            }
        }
    }

    private void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        if (hitEffectPrefab != null)
        {
            Debug.Log($"Spawning hit effect at {position}");
            Instantiate(hitEffectPrefab, position, Quaternion.LookRotation(normal));
        }
        else
        {
            // Dočasné vizuální ověření
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = position;
            sphere.transform.localScale = Vector3.one * 0.1f; // Zmenšení
            Destroy(sphere, 2f); // Sphere zmizí po 2 sekundách
            Debug.LogWarning("Using test sphere because hitEffectPrefab is not assigned!");
        }
    }
    public void UpdateToolDamage(float damageMultiplier)
    {
        // Aplikujte modifikátor poškození, například z perků
        damage *= damageMultiplier;
    }
    private void ApplyLifesteal(float lifesteal)
    {
        // Získání hodnoty lifestealu z perků nebo jiného nastavení
        float lifestealAmount = damage * lifesteal; // Např. 0.1 pro 10% lifestealu
        player.currentHealth += lifestealAmount;

        // Omezte zdraví, pokud by překročilo maximální limit
        if (player.currentHealth > player.maxHealth)
        {
            player.currentHealth = player.maxHealth;
        }

        Debug.Log($"Lifesteal applied: {lifestealAmount}, Current Health: {player.currentHealth}");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
