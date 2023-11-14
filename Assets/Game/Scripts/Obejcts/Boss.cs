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

    Rigidbody2D rigid; // ���� �Է��� �ޱ����� ����

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool shouldBeAttack = 0 >= attackDelayTimer; // ���� ��Ÿ���� �ƴ��� Ȯ��
        if (shouldBeAttack)
        {
            attackDelayTimer = skillDelay;

            TryAttack(); // ��ų ��Ÿ���� �� ������ ������ �õ��Ѵ�
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
        yield return new WaitForSeconds(0.3f); // ������ �� ��ŭ ����

        onBossTryAttack();
        animator.SetBool("Shoot", false);
    }

    // IDamageable�� �Լ� TakeDamage
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
        //onBossWasKilled(this); // �븮�� ȣ��

        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.5f); // ������ �� ��ŭ ����

        Destroy(gameObject);
    }
}
