using System.Collections;
using UnityEngine;

public class Boss_Bullet : Object
{
    public float damage = 10f;
    Animator animator;

    bool isDead;

    private float aliveTime; // 스킬 생존 시간을 체크할 변수

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(!isDead) 
        {
            X += 10f * Time.deltaTime;

            if (X >= 5)
            {
                bool destroySkill = aliveTime > 0.3f;

                if (destroySkill)
                {
                    StartCoroutine(Dead());
                    StopCoroutine(Dead());
                    return;
                }

                aliveTime += Time.deltaTime;
            }
        }
    }

    IEnumerator Dead()
    {
        animator.SetTrigger("Hit");

        isDead = true;

        yield return new WaitForSeconds(0.35f); // 지정한 초 만큼 쉬기

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        iPlayer.TakeDamage(damage);
    }
}

