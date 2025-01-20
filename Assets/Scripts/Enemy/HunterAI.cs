using UnityEngine;

public class HunterAI : EnemyAI
{
    public GameObject rangedProjectilePrefab;  // Prefab projektilu
    public Transform shootingPoint;            // Bod, odkud se vystøelí projektil
    public float projectileSpeed = 10f;        // Rychlost projektilu          // Rozsah útoku
    public float meleeAttackRange = 2f;        // Rozsah pro melee útok
    public float meleeDamage = 10f;            // Poškození melee útoku
    protected new float attackCooldown = 1f;          // Èas mezi útoky

    private void Start()
    {
        base.Start();
        attackRange = 20f;
    }
    private new void Update()
    {
        base.Update(); // Voláme základní Update() metodu z EnemyAI

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            // Pokud je hráè v dostateèné vzdálenosti pro ranged útok
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                ShootProjectile();
            }
        }

        // Pokud je hráè blízko pro melee útok
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
        // Vytvoøíme projektil
        GameObject projectile = Instantiate(rangedProjectilePrefab, shootingPoint.position, Quaternion.identity);

        // Pohyb projektilu smìrem k hráèi
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - shootingPoint.position).normalized; // Spoèítáme smìr
            rb.velocity = direction * projectileSpeed; // Nastavíme rychlost pohybu
        }
    }

    private void MeleeAttack()
    {
        // Mùžeš zde pøidat nìjakou animaci nebo efekt útoku
        Debug.Log("Hunter is melee attacking!");

        // Poškození hráèi, pokud je v dostateèné blízkosti
        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(meleeDamage);
        }
    }
}
