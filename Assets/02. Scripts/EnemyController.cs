using UnityEngine;

public class EnemyController : BaseCharacter
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.0f;
    public int attackDamage = 10;

    private Transform target;
    private float lastAttackTime;

    private void Start()
    {
        target = GameManager.Instance.PlayerManager.Player.transform;
    }

    private void Update()
    {
        if (IsDead || target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange)
        {
            MoveTowardTarget();
        }
        else if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void MoveTowardTarget()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(target);
    }

    private void Attack()
    {
        if (target.TryGetComponent<ICharacter>(out var character))
        {
            character.TakeDamage(attackDamage);
        }
    }

    public override void Die()
    {
        base.Die();
        GameManager.Instance.SaveManager.AddMoney(10); // 예시: 돈 획득
    }
    
    public override void TakeDamage(int amount)
    {
        // Debug.Log("몬스터 히트!");
        base.TakeDamage(amount);
    }
}