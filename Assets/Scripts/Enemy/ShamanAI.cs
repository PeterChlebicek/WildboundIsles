using UnityEngine;

public class ShamanAI : EnemyAI
{
    public GameObject spikePrefab;
    public float spikeCooldown = 5f;

    protected override void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            base.Attack();
            UseSpikeAttack();
        }
    }

    private void UseSpikeAttack()
    {
        Instantiate(spikePrefab, player.position, Quaternion.identity);
    }
}
