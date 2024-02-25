using System.Collections;
using UnityEngine;

public class Boss_Genesis : BossSkill, IPullingObject
{
    //public float impactPonitX;
    //public float impactPonitY;

    private float aliveTimer; // 스킬 생존 시간을 체크할 변수
    public float aliveTime; // 스킬 생존 시간
    private bool isCoroutineNow; // 현재 코루틴을 실행하고 있는지를 체크할 변수

    public bool isStaySkill; // 몇초동안 지속되다가 사라지는 스킬이냐

    Animator animator;

    SpriteRenderer spriteRenderer_child1;
    SpriteRenderer spriteRenderer_child2;
    SpriteRenderer spriteRenderer_child3;
    SpriteRenderer spriteRenderer_child4;

    public new void Init()
    {
        aliveTimer = 0f;
        isCoroutineNow = false;
        animator.SetBool("Stay", true);

        UnityEngine.Color col = spriteRenderer_child1.color;
        col.a = 1f;
        spriteRenderer_child1.color = col;

        col = spriteRenderer_child2.color;
        col.a = 1f;
        spriteRenderer_child2.color = col;

        col = spriteRenderer_child3.color;
        col.a = 1f;
        spriteRenderer_child3.color = col;

        col = spriteRenderer_child4.color;
        col.a = 1f;
        spriteRenderer_child4.color = col;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Stay", true);
        spriteRenderer_child1 = GetComponentsInChildren<SpriteRenderer>()[0];
        spriteRenderer_child2 = GetComponentsInChildren<SpriteRenderer>()[1];
        spriteRenderer_child3 = GetComponentsInChildren<SpriteRenderer>()[2];
        spriteRenderer_child4 = GetComponentsInChildren<SpriteRenderer>()[3];

        damage = 0.25f;
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            if (!isCoroutineNow)
                StartCoroutine(Disappear());
            
        }

        aliveTimer += Time.fixedDeltaTime;
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        iPlayer.TakeDamage(damage);
    }

    IEnumerator Disappear()
    {
        animator.SetTrigger("Exit");

        isCoroutineNow = true;

        for(float i = 0.99f; i > 0;)
        {
            UnityEngine.Color col = spriteRenderer_child1.color;
            col.a = i;
            spriteRenderer_child1.color = col;

            col = spriteRenderer_child2.color;
            col.a = i;
            spriteRenderer_child2.color = col;

            col = spriteRenderer_child3.color;
            col.a = i;
            spriteRenderer_child3.color = col;

            col = spriteRenderer_child4.color;
            col.a = i;
            spriteRenderer_child4.color = col;

            i -= 0.01f;

        }

        yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기

        GameManager.instance.poolManager.ReturnBossSkill(this, index);

        isCoroutineNow = false;
    }
}