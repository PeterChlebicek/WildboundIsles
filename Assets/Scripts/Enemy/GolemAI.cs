using UnityEngine;

public class GolemAI : EnemyAI
{
    public float areaAttackRadius = 5f; // Polom�r area �toku
    public float areaAttackDamage = 30f; // Po�kozen� area �toku
    public float areaAttackCooldown = 10f; // Cooldown area �toku
    private float lastAreaAttackTime;

    protected override void Start()
    {
        base.Start();
        lastAreaAttackTime = Time.time - areaAttackCooldown; // Aby mohl �tok prov�st ihned na za��tku
    }

    protected override void Attack()
    {
        base.Attack(); // Klasick� melee �tok

        // Pokud je area �tok dostupn�, pou�ij jej
        if (Time.time >= lastAreaAttackTime + areaAttackCooldown)
        {
            PerformAreaAttack();
            lastAreaAttackTime = Time.time;
        }
    }


    private void PerformAreaAttack()
    {
        Debug.Log("Golem used an area attack!");

        // Najdi v�echny objekty v radiusu
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, areaAttackRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Player playerScript = hitCollider.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(areaAttackDamage);
                    Debug.Log("Player took damage from Golem's area attack!");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Vizualizace oblasti �toku v editoru Unity
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaAttackRadius);
    }
}
