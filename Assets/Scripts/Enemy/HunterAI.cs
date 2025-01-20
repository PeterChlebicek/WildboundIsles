using UnityEngine;

public class HunterAI : EnemyAI
{
    public GameObject rangedProjectilePrefab;  // Prefab projektilu
    public Transform shootingPoint;            // Bod, odkud se vyst�el� projektil
    public float projectileSpeed = 10f;        // Rychlost projektilu          // Rozsah �toku
    public float meleeAttackRange = 2f;        // Rozsah pro melee �tok
    public float meleeDamage = 10f;            // Po�kozen� melee �toku
    protected new float attackCooldown = 1f;          // �as mezi �toky

    private void Start()
    {
        base.Start();
        attackRange = 20f;
    }
    private new void Update()
    {
        base.Update(); // Vol�me z�kladn� Update() metodu z EnemyAI

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            // Pokud je hr�� v dostate�n� vzd�lenosti pro ranged �tok
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                ShootProjectile();
            }
        }

        // Pokud je hr�� bl�zko pro melee �tok
        if (Vector3.Distance(transform.position, player.position) <= meleeAttackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                MeleeAttack();
            }
        }
    }

    private void ShootProjectile()
    {
        // Vytvo��me projektil
        GameObject projectile = Instantiate(rangedProjectilePrefab, shootingPoint.position, Quaternion.identity);

        // Pohyb projektilu sm�rem k hr��i
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - shootingPoint.position).normalized; // Spo��t�me sm�r
            rb.velocity = direction * projectileSpeed; // Nastav�me rychlost pohybu
        }
    }

    private void MeleeAttack()
    {
        // M��e� zde p�idat n�jakou animaci nebo efekt �toku
        Debug.Log("Hunter is melee attacking!");

        // Po�kozen� hr��i, pokud je v dostate�n� bl�zkosti
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(meleeDamage);
        }
    }
}
