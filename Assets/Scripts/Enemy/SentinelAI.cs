using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelAI : EnemyAI
{
    private bool isUnyielding;

    protected override void SpecialAbility()
    {
        Debug.Log("Sentinel used Unyielding Resolve!");
        isUnyielding = true;
        Invoke(nameof(EndUnyielding), 5f); // Trvání 5 sekund
    }

    private void EndUnyielding()
    {
        isUnyielding = false;
        Debug.Log("Sentinel ended Unyielding Resolve.");
    }

    public override void HurtEnemy(float damage)
    {
        if (!isUnyielding)
        {
            base.HurtEnemy(damage);
        }
        else
        {
            Debug.Log("Sentinel is invincible!");
        }
    }
}

