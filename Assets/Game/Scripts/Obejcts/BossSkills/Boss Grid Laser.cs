using System.Collections;
using UnityEngine;

public class Boss_Grid_Lazer : Object
{
    public Player player;
    public Boss boss;
    private float damage = 0.2f;

    public bool isRightTop;
    public bool isRightBottom;

    public float aliveTime = 4f; // 스킬 생존 시간을 체크할 변수
    private float aliveTimer; // 스킬 생존 시간 타이머

    private float safeTime = 1.6f; // 데미지 안들어가는 시간
    private float safeTimer; // 데미지 안들어가는 시간 타이머

    public float laserTurnNum; // 레이저 회전 각도

    private bool isCoroutineNow; // 현재 코루틴을 실행하고 있는지를 체크할 변수

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Stay", false);

        float angle = 0f;
        if (isRightTop)
        {
            angle = 45f;
        }
        else if(isRightBottom)
        {
            angle = -45f;
        }

        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
        transform.rotation = rotation;
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill && !isCoroutineNow)
        {
            StartCoroutine(Disappear());
            return;
        }

        if(safeTimer >= safeTime)
        {
            animator.SetBool("Stay", true);
        }

        aliveTimer += Time.fixedDeltaTime;
        safeTimer += Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        if (safeTimer >= safeTime)
        {
            iPlayer.TakeDamage(damage);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        if (safeTimer >= safeTime)
        {
            iPlayer.TakeDamage(damage);
        }
    }

    IEnumerator Disappear()
    {
        animator.SetTrigger("Exit");

        isCoroutineNow = true;

        yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기

        Destroy(gameObject);
    }
}

