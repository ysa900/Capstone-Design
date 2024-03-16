using System;
using System.Collections;
using UnityEngine;

public class RandomSkill : Skill, IPullingObject
{
    public RandomSkill randomSkill;

    public float impactPonitX;
    public float impactPonitY;

    private float aliveTimer; // 스킬 생존 시간을 체크할 변수
    public float aliveTime; // 스킬 생존 시간
    private bool isCoroutineNow; // 현재 코루틴을 실행하고 있는지를 체크할 변수

    public bool isMeteor; // 메테오면 날아오는 도중엔 데미지 없어야 하므로 만든 변수
    public bool isIceSpike; // IceSpike 스킬의 자식 오브젝트 때문에 만든 변수
    public bool isStaySkill; // 몇초동안 지속되다가 사라지는 스킬이냐

    public float scale;

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    Vector2 direction; // 날아갈 방향

    Animator animator;
    Animator animator_ground;

    public new void Init()
    {
        aliveTimer = 0;
        if (isIceSpike)
        {
            animator_ground = transform.Find("ground").GetComponent<Animator>();
        }
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (isIceSpike)
        {
            animator_ground = transform.Find("ground").GetComponent<Animator>();
        }
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            if (isMeteor)
            {
                RandomSkill explode;
                explode = GameManager.instance.poolManager.GetSkill(2) as RandomSkill;

                explode.X = X;
                explode.Y = Y;

                explode.aliveTime = 0.5f;
                explode.damage = damage;

                Transform parent = explode.transform.parent;

                explode.transform.parent = null;
                explode.transform.localScale = new Vector3(scale, scale, 0);
                explode.transform.parent = parent;
            }
            if (isStaySkill)
            {
                if(!isCoroutineNow)
                    StartCoroutine(Disappear());
            }
            else
            {
                GameManager.instance.poolManager.ReturnSkill(this, index);
            }
            
            return;
        }
        else
        {
            if(isMeteor) { MoveToimpactPonit(); }
        }

        aliveTimer += Time.fixedDeltaTime;
    }

    // 날아갈 방향을 정하는 함수
    public void setDirection()
    {
        Vector2 impactVector = new Vector2 (impactPonitX, impactPonitY);
        Vector2 nowVector = transform.position;

        direction = impactVector - nowVector;

        direction = direction.normalized;
    }

    // 폭발 지점으로 이동하는 함수
    private void MoveToimpactPonit()
    {
        setDirection();

        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);

        X = transform.position.x;
        Y = transform.position.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isMeteor)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(gameObject, damage);

                return;
            }

            IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();

            if (damageableSkill != null)
            {
                damageableSkill.TakeDamage(damage);

                return;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isMeteor && isStaySkill)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(gameObject, damage);

                return;
            }

            IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();

            if (damageableSkill != null)
            {
                damageableSkill.TakeDamage(damage);

                return;
            }
        }
    }

    IEnumerator Disappear()
    {
        animator.SetTrigger("Finish");
        animator_ground.SetTrigger("Finish");

        isCoroutineNow = true;
        
        yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기
        
        GameManager.instance.poolManager.ReturnSkill(this, index);

        isCoroutineNow = false;
    }
}