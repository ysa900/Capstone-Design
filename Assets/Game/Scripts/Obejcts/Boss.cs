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
}
