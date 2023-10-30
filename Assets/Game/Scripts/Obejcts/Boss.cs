using System.Collections;
using UnityEngine;

public class Boss : Object
{
    public Player player;

    float attackDelayTimer = 1f;
    float skillDelay = 2f;

    Animator animator;

    public delegate void OnBossTryAttack();
    public OnBossTryAttack onBossTryAttack;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
}
