using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Object
{
    // 플레이어 객체
    public Player player;

    // 보스 정보
    public int hp;
    public float speed;
    private float colliderOffsetX; // collider의 offset x좌표
    private float colliderOffsetY; // collider의 offset y좌표


    //Boss가 죽었을 때 BossManager에게 알려주기 위한 delegate
    public delegate void OnBossWasKilled(Boss killedBoss);
    public OnBossWasKilled onBossWasKilled;

    // 물리 입력을 받기위한 변수
    Rigidbody2D rigid;

    SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수

    Animator animator; // 애니메이션 관리를 위한 변수

    CapsuleCollider2D capsuleCollider; // Collider의 offset을 변경하기 위한 변수

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        colliderOffsetX = capsuleCollider.offset.x; // offset 초기값을 저장
        colliderOffsetY = capsuleCollider.offset.y;
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

        bool isBossLookLeft = direction.x < 0;
        spriteRenderer.flipX = isBossLookLeft;

        Vector2 colliderOffset; // CapsuleCollider의 offset에 넣을 Vector2

        if (isBossLookLeft) // Enemy가 왼쪽을 보면 collider도 x축 대칭을 해준다
        {
            colliderOffset = new Vector2(-colliderOffsetX, colliderOffsetY);
        }
        else
        {
            colliderOffset = new Vector2(colliderOffsetX, colliderOffsetY);

        }
        capsuleCollider.offset = colliderOffset; // capsuleCollider에 적용

        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);
    }

    // IDamageable의 함수 TakeDamage
    public void TakeDamage(GameObject causer, float damage)
    {
        hp = hp - (int)damage;

        if (hp < 0)
        {
            Debug.Log("보스 사망");

            animator.SetBool("Dead", true);

            //  대리자 호출
            onBossWasKilled(this);

            Destroy(gameObject);
        }

    }

}
