using System.Collections;
using UnityEngine;

public class Boss : Object, IDamageable
{
    int hp = 1000;

    public Player player;

    float attackDelayTimer = 1f;
    float skillDelay = 2f;

    Animator animator;

    public delegate void OnBossTryAttack();
    public OnBossTryAttack onBossTryAttack;

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool shouldBeAttack = 0 >= attackDelayTimer; // 공격 쿨타임이 됐는지 확인
        if (shouldBeAttack)
        {
            attackDelayTimer = skillDelay;

            TryAttack(); // 스킬 쿨타임이 다 됐으면 공격을 시도한다
        }
        else
        {
            attackDelayTimer -= Time.deltaTime;
        }
    }

    private void TryAttack()
    {
        StartCoroutine(Wait());
        StopCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        animator.SetBool("Shoot", true);
        yield return new WaitForSeconds(0.3f); // 지정한 초 만큼 쉬기

        onBossTryAttack();
        animator.SetBool("Shoot", false);
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
    }

    IEnumerator Dead()
    {
        animator.SetTrigger("Dead");
        //onBossWasKilled(this); // 대리자 호출

        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.5f); // 지정한 초 만큼 쉬기

        Destroy(gameObject);
    }
}
