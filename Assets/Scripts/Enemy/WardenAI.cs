using System.Collections;
using UnityEngine;

public class WardenAI : EnemyAI
{
    [Header("Warden Specific Settings")]
    public float rootChance = 0.3f; // �ance na pou�it� Root of Eternity
    public float rootDuration = 3f; // Doba znehybn�n� hr��e

    protected override void Attack()
    {
        // Melee �tok
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                Debug.Log("Warden strikes the player!");
                player.GetComponent<Player>().TakeDamage(damage);

                // �ance na pou�it� Root of Eternity
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
            playerScript.ApplyRoot(rootDuration); // Znehybn� hr��e na ur�itou dobu
        }
    }
}
