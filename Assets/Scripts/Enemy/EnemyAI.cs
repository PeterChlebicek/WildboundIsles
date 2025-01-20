using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine;

public enum EnemyState { Wander, Chase, Attack }

public class EnemyAI : MonoBehaviour
{
    public EnemyState currentState;
    protected Transform player;

    [Header("Enemy Settings")]
    public float maxHealth = 100f;
    public float health;
    public float damage;
    public float chaseRange;
    public float wanderSpeed;
    public float chaseSpeed;
    public float attackRange;
    public float attackCooldown;
    public float wanderRadius = 15f;
    public float lastAttackTime;
    private NavMeshAgent agent;
    private Vector3 wanderTarget;
    private float wanderTimer = 0f;
    public float wanderInterval = 3f;

    [Header("Enemy Healthbar")]
    public Image healthBar;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastAttackTime = Time.time;
        health = maxHealth;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player object has the tag 'Player'.");
        }
    }

    protected virtual void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case EnemyState.Wander:
                Wander();
                if (Vector3.Distance(transform.position, player.position) < chaseRange)
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                Chase();
                if (Vector3.Distance(transform.position, player.position) < attackRange)
                    currentState = EnemyState.Attack;
                else if (Vector3.Distance(transform.position, player.position) > chaseRange)
                    currentState = EnemyState.Wander;
                break;

            case EnemyState.Attack:
                Attack();
                if (Vector3.Distance(transform.position, player.position) > attackRange)
                    currentState = EnemyState.Chase;
                break;
        }
    }
    public void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = health / maxHealth;
        }
    }

    public virtual void HurtEnemy(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0)
        {
            DieEnemy();
        }
    }

    protected virtual void DieEnemy()
    {
        Destroy(gameObject);
    }

    protected virtual void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
                Debug.Log("Enemy attacked the player!");
            }
        }
    }

    protected virtual void SpecialAbility()
    {
        // Special ability logic to be overridden
    }

    protected void Wander()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderInterval || Vector3.Distance(transform.position, wanderTarget) < 1f)
        {
            SetNewWanderTarget();
            wanderTimer = 0f;
        }

        agent.SetDestination(wanderTarget);
        agent.speed = wanderSpeed;
    }

    protected void SetNewWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
        }
    }

    protected virtual void Chase()
    {
        agent.SetDestination(player.position);
        agent.speed = chaseSpeed;
    }
}
