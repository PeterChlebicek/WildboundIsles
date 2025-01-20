using UnityEngine;

public class PerkPickup : MonoBehaviour
{
    public Perk perk; // Odkaz na ScriptableObject perku
    private bool playerInRange = false;
    private PerkChest parentChest; // Odkaz na bednu (parent objekt)

    void Start()
    {
        // Najdi rodièovskou bednu, pokud existuje
        parentChest = GetComponentInParent<PerkChest>();
    }

    void Update()
    {
        // Pokud je hráè v dosahu a stiskne klávesu E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PerkManager perkManager = FindObjectOfType<PerkManager>(); // Najdi PerkManager ve scénì
            if (perkManager != null && perk != null)
            {
                perkManager.ApplyPerk(perk); // Aplikuj perk na hráèe
                Debug.Log($"Player collected perk: {perk.perkName}");

                Destroy(gameObject); // Zniè objekt perku
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Pokud hráè vstoupí do oblasti
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Pokud hráè opustí oblast
        {
            playerInRange = false;
        }
    }
}
