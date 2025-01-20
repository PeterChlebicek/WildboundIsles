using System.Collections;
using UnityEngine;

public class WardenAI : EnemyAI
{
    [Header("Warden Specific Settings")]
    public float rootChance = 0.3f; // Šance na použití Root of Eternity
    public float rootDuration = 3f; // Doba znehybnìní hráèe

    protected override void Attack()
    {
        // Melee útok
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                Debug.Log("Warden strikes the player!");
                player.GetComponent<Player>().TakeDamage(damage);

                // Šance na použití Root of Eternity
                if (Random.value < rootChance)
                {
                    PerformRootOfEternity();
                }
            }
        }
    }

    private void PerformRootOfEternity()
    {
        Debug.Log("Warden uses Root of Eternity!");

        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.ApplyRoot(rootDuration); // Znehybní hráèe na urèitou dobu
        }
    }
}
