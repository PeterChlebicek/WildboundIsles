using UnityEngine;

public class GolemAI : EnemyAI
{
    public float areaAttackRadius = 5f; // Polomìr area útoku
    public float areaAttackDamage = 30f; // Poškození area útoku
    public float areaAttackCooldown = 10f; // Cooldown area útoku
    private float lastAreaAttackTime;

    protected override void Start()
    {
        base.Start();
        lastAreaAttackTime = Time.time - areaAttackCooldown; // Aby mohl útok provést ihned na zaèátku
    }

    protected override void Attack()
    {
        base.Attack(); // Klasický melee útok

        // Pokud je area útok dostupný, použij jej
        if (Time.time >= lastAreaAttackTime + areaAttackCooldown)
        {
            PerformAreaAttack();
            lastAreaAttackTime = Time.time;
        }
    }


    private void PerformAreaAttack()
    {
        Debug.Log("Golem used an area attack!");

        // Najdi všechny objekty v radiusu
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
        // Vizualizace oblasti útoku v editoru Unity
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaAttackRadius);
    }
}
