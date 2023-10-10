using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Skill : Object
{
    public Enemy enemy;

    public float speed;
    public float damage;

    public bool isBulletSkill; // 적에게 총알처럼 날아가는 스킬이냐
    public bool isEnemyOnSkill; // 적 머리 위에 떨어지는 스킬이냐

    private float gameTime; // 스킬 생존 시간을 체크할 변수

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        bool destroySkill = gameTime > 1f || enemy == null;

        if (destroySkill)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            if (isBulletSkill) { MoveToEnemy(); } // 날아가는 스킬이면 실행
        }
        
      
        gameTime += Time.fixedDeltaTime;
    }

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

        if (isEnemyOnSkill) { 
            StartCoroutine(Delay());
            StopCoroutine(Delay());
        }
        else { Destroy(gameObject); }
    }

    
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f); // 지정한 초 만큼 쉬기
        Destroy(gameObject);
    }
}

