using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy : Object
{
    // 플레이어 객체
    public Player player;

    // Enemy 정보
    public float hp;
    public float maxHp;
    public float speed;

    // Enemy 살아있는지 확인 위한 변수
    bool isEnemyLive = true;

    
    // enemy가 죽었을 때 EnemyManager에게 알려주기 위한 delegate
    public delegate void OnEnemyWasKilled(Enemy killedEnemy);
    public OnEnemyWasKilled onEnemyWasKilled;
    
    Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    Collider2D collider;

    SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수
    Animator animator; // 애니메이션 관리를 위한 변수

    WaitForFixedUpdate waitFixedUpdateTime;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        waitFixedUpdateTime=new WaitForFixedUpdate();
    }

    private void FixedUpdate()
    {
        // HitEnemyByPlayer()를 원활하기 위해 GetCurrentAnimatorStateInfo 이용
        // GetCurrentAnimatorStateInfo: 현재 상태 정보를 가져오는 함수
        if (!isEnemyLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
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
    /*
    // OnEnable: Enemy 생성되면서 변수를 초기화해주는 함수
    void OnEnable()
    {
        rigid = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isEnemyLive = true;
        collider.enabled = true; // 컴포넌트의 비활성화 방법: .enabled = false
        rigid.simulated = true; // rigidBody의 물리적 비활성화 방법: .simulated = false
        spriteRenderer.sortingOrder = 2;
        animator.SetBool("Dead", false);
        hp = maxHp;
    }
    */

    // IDamageable의 함수 TakeDamage
    public void TakeDamage(GameObject causer, float damage)
    {
        animator.SetBool("Hit", true);

        hp = hp - (int)damage;
        StartCoroutine(HitEnemyByPlayer());

        animator.SetBool("Hit", false);

        if (hp <= 0)
        {
            // Debug.Log("적군 사망");
            isEnemyLive = false;
            collider.enabled = false; // 컴포넌트의 비활성화 방법: .enabled = false
            rigid.simulated = false; // rigidBody의 물리적 비활성화 방법: .simulated = false
            spriteRenderer.sortingOrder = 1;
            animator.SetBool("Dead", true);

            //  대리자 호출
            onEnemyWasKilled(this);

            Destroy(gameObject); // 사망한 Enemy 오브젝트 지우기
        } else
        {
            animator.SetTrigger("Hit");
        }
    }

    // Enemy가 피격됐을 때 실행(NockBack 효과)
    // 코루틴 Coroutine: 생명 주기와 비동기처럼 실행되는 함수
    // IEnumerator: 코루틴만의 반환형 인터페이스
    IEnumerator HitEnemyByPlayer()
    {
        yield return waitFixedUpdateTime; // 다음 하나의 물리 프레임을 딜레이

        Vector3 playerPosition=player.transform.position; // 플레이어 포지션
        Vector3 directionVector = transform.position - playerPosition; // 방향
        rigid.AddForce(directionVector.normalized * 3, ForceMode2D.Impulse);
    }

}