using UnityEngine;

public class Skill : Object
{
    public Enemy enemy;

    public float speed;
    public float damage;

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수


    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (enemy == null)
        {
            return;
        }

        MoveToEnemy();
    }

    private void MoveToEnemy()
    {
        Vector2 enemyPosition = enemy.transform.position;
        Vector2 myPosition = transform.position;

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

