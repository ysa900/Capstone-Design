using System.Collections;
using UnityEngine;

public class Enemy : Object, IDamageable
{
    // 플레이어 객체
    public Player player;

    // enemy 정보
    public int hp;
    public float speed;
    private float colliderOffsetX; // collider의 offset x좌표
    private float colliderOffsetY; // collider의 offset y좌표

    public bool isEnemyLookLeft; // 적이 보고 있는 방향을 알려주는 변수

    private bool isDead;

    // enemy가 죽었을 때 EnemyManager에게 알려주기 위한 delegate
    public delegate void OnEnemyWasKilled(Enemy killedEnemy);
    public OnEnemyWasKilled onEnemyWasKilled;

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수

    Animator animator; // 애니메이션 관리를 위한 변수

    public CapsuleCollider2D capsuleCollider; // Collider의 offset을 변경하기 위한 변수

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
        if (!isDead)
        {
            MoveToPlayer();
        }
    }

    // 플레이어 방향으로 이동하는 함수
    private void MoveToPlayer()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 myPosition = transform.position;

        Vector2 direction = playerPosition - myPosition;
        direction = direction.normalized;

        isEnemyLookLeft = direction.x < 0;
        spriteRenderer.flipX = isEnemyLookLeft;

        Vector2 colliderOffset; // CapsuleCollider의 offset에 넣을 Vector2

        if (isEnemyLookLeft) // Enemy가 왼쪽을 보면 collider도 x축 대칭을 해준다
        {
            colliderOffset = new Vector2(-colliderOffsetX, colliderOffsetY);
        }
        else {
            colliderOffset = new Vector2(colliderOffsetX, colliderOffsetY);

        }
        capsuleCollider.offset = colliderOffset; // capsuleCollider에 적용

        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime); // 플레이어 방향으로 위치 변경
    }

    // IDamageable의 함수 TakeDamage
    public void TakeDamage(GameObject causer, float damage)
    {
        hp = hp - (int)damage;

        if (hp <= 0)
        {
            StartCoroutine(Dead());
            StopCoroutine(Dead());
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    IEnumerator Dead()
    {
        animator.SetBool("Dead", true);
        onEnemyWasKilled(this); // 대리자 호출

        isDead = true;

        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.5f); // 지정한 초 만큼 쉬기

        Destroy(gameObject);
    }

}