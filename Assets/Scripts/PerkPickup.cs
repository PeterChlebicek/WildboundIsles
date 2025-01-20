using UnityEngine;

public class PerkPickup : MonoBehaviour
{
    public Perk perk; // Odkaz na ScriptableObject perku
    private bool playerInRange = false;
    private PerkChest parentChest; // Odkaz na bednu (parent objekt)

    void Start()
    {
        // Najdi rodi�ovskou bednu, pokud existuje
        parentChest = GetComponentInParent<PerkChest>();
    }

    void Update()
    {
        // Pokud je hr�� v dosahu a stiskne kl�vesu E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PerkManager perkManager = FindObjectOfType<PerkManager>(); // Najdi PerkManager ve sc�n�
            if (perkManager != null && perk != null)
            {
                perkManager.ApplyPerk(perk); // Aplikuj perk na hr��e
                Debug.Log($"Player collected perk: {perk.perkName}");

                Destroy(gameObject); // Zni� objekt perku
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Pokud hr�� vstoup� do oblasti
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Pokud hr�� opust� oblast
        {
            playerInRange = false;
        }
    }
}
