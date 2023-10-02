﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy : Object
{
    // 플레이어 객체
    public Player player;

    // enemy 정보
    public int hp;
    public float speed;

    // enemy가 죽었을 때 EnemyManager에게 알려주기 위한 delegate
    public delegate void OnEnemyWasKilled(Enemy killedEnemy);
    public OnEnemyWasKilled onEnemyWasKilled;

    // 물리 입력을 받기위한 변수
    Rigidbody2D rigid;

    SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수

    Animator animator; // 애니메이션 관리를 위한 변수

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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

        bool isEnemyLookLeft = direction.x < 0;
        spriteRenderer.flipX = isEnemyLookLeft;

        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);
    }

    // IDamageable의 함수 TakeDamage
    public void TakeDamage(GameObject causer, float damage)
    {
        animator.SetBool("Hit", true);

        hp = hp - (int)damage;

        animator.SetBool("Hit", false);

        if (hp < 0)
        {
            Debug.Log("적군 사망");

            animator.SetBool("Dead", true);

            //  대리자 호출
            onEnemyWasKilled(this);

            Destroy(gameObject);
        }
    }
}