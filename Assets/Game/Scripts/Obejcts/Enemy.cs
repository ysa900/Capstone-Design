using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy : Object, IDamageable
{
    // 플레이어 객체
    public Player player;

    // enemy 정보
    public int hp;
    public float speed;
    public int damage;

    // enemy가 죽었을 때 EnemyManager에게 알려주기 위한 delegate
    public delegate void OnEnemyWasKilled(Enemy killedEnemy);
    public OnEnemyWasKilled onEnemyWasKilled;

    // 물리 입력을 받기위한 변수
    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        MoveToPlayer();
    }

    // 플레이어 방향으로 이동하는 함수
    private void MoveToPlayer()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 myPosition = transform.position;

        Vector2 direction = playerPosition - myPosition;
        direction = direction.normalized;

        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);
    }

    // IDamageable의 함수 TakeDamage
    public void TakeDamage(GameObject causer, float damage)
    {
        hp = hp - (int)damage;

        if (hp < 0)
        {
            Debug.Log("적군 사망");

            //  대리자 호출
            onEnemyWasKilled(this);

            Destroy(gameObject);
        }
    }

    // 다른 오브젝트랑 닿으면 실행
    // 이거 지금 안됨, OnCollisionEnter로 해야될듯, OnTriggerEnter는 통과되고, OnCollision은 통과 안됨
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        Debug.Log("dd");

        if (damageable == null)
        {
            return;
        }

        damageable.TakeDamage(gameObject, damage);
    }
}