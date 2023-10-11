using UnityEngine;

public class EnemyTrackingSkill : Skill
{
    private float aliveTime; // 스킬 생존 시간을 체크할 변수

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTime > 1f || enemy == null;

        if (destroySkill)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            MoveToEnemy();
        }

        aliveTime += Time.fixedDeltaTime;
    }

    // 적을 따라가는 스킬
    private void MoveToEnemy()
    {
        Vector2 enemyPosition = enemy.transform.position;
        Vector2 myPosition = transform.position;

        // 적 실제 위치로 보정
        if (enemy.isEnemyLookLeft)
            enemyPosition.x -= enemy.capsuleCollider.size.x * 5;
        else
            enemyPosition.x += enemy.capsuleCollider.size.x * 5;
        enemyPosition.y += enemy.capsuleCollider.size.y * 5;

        Vector2 direction = enemyPosition - myPosition;

        direction = direction.normalized;

        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime); // Enemy 방향으로 위치 변경
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable == null)
        {
            return;
        }

        damageable.TakeDamage(gameObject, damage);

        Destroy(gameObject);
    }
}

