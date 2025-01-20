using UnityEngine;

public class GhoulAI : EnemyAI
{
    public float healAmount = 5f;

    protected override void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            base.Attack();
            HealGhoul();
        }
    }

    private void HealGhoul()
    {
        health += healAmount;
        health = Mathf.Clamp(health, 0, maxHealth); // Regenerace až do max hodnoty zdraví
        UpdateHealthBar();
    }
}
