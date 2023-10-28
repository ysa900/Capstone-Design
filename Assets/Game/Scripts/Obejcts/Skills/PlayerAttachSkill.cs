using System;
using UnityEngine;

public class PlayerAttachSkill : Skill
{
    // 플레이어 좌표를 기준으로 위치를 어디로 가야하나를 받는 변수
    public float xPositionNum;
    public float yPositionNum;

    public bool isAttachSkill; // 플레이어 근처에 붙어다니는 스킬이냐
    public bool isCircleSkill; // 플레이어 주위를 빙빙 도는 스킬이냐
    public bool isShieldSkill; // Damage가 없는 Shield 스킬이냐

    private float degree = 0f;
    private float tmpX; // Cirle을 계산할 때 0,0을 기준으로 생각한 X
    private float tmpY; // Cirle을 계산할 때 0,0을 기준으로 생각한 X
    private bool isUpSide; // 플레이어를 돌 때 반원 기준으로 위에 있는지 아래 있는지를 판단할 변수

    private float aliveTime; // 스킬 생존 시간을 체크할 변수
    private float aliveTimer; // 스킬 생존 시간
    SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수

    public delegate void OnShieldSkillDestroyed();
    public OnShieldSkillDestroyed onShieldSkillDestroyed;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (isCircleSkill) { aliveTimer = 5f; }
        else if (isAttachSkill) { aliveTimer = 0.5f; }
        else { aliveTimer = 3f; }
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTime > aliveTimer;

        if (destroySkill)
        {
            if (isShieldSkill)
                onShieldSkillDestroyed(); // 쉴드 스킬이 파괴될 땐 SkillManager에 알려준다

            Destroy(gameObject);
            return;
        }
        else if (isCircleSkill)
        {
            CircleMove();
        }
        else if (isAttachSkill)
        {
            AttachPlayer();
        }
        else
        {
            X = player.transform.position.x;
            Y = player.transform.position.y;
        }

        aliveTime += Time.fixedDeltaTime;
    }

    // 플레이어에 붙어다니는 스킬
    private void AttachPlayer()
    {
        if (player.isPlayerLookLeft)
        {
            spriteRenderer.flipX = !player.isPlayerLookLeft; // 반대로 하는 이유는 기본 프리팹이 방향이 반대라서 (나중에 일관성있게 바꿀 필요 있음)
            X = player.gameObject.transform.position.x - xPositionNum;
        }
        else
        {
            spriteRenderer.flipX = !player.isPlayerLookLeft;
            X = player.gameObject.transform.position.x + xPositionNum;
        }

        Y = player.gameObject.transform.position.y + yPositionNum;
    }

    // 플레이어 주위를 빙빙 도는 스킬
    private void CircleMove()
    {
        degree += 0.1f;

        if (degree >= 180)
        {
            isUpSide ^= false;
        }

        tmpX = (float)Math.Cos(degree) * xPositionNum;
        tmpY = (float)Math.Sin(degree) * xPositionNum; //이거 잘못쓴거 아님 (xPositionNum이 여기서 반지름 역할)

        if (!isUpSide) // 아래 반원이면 음수값 부여
            tmpY = -tmpY;

        X = tmpX + player.transform.position.x;
        Y = tmpY + player.transform.position.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable == null)
        {
            return;
        }

        if (!isShieldSkill)
            damageable.TakeDamage(gameObject, damage);
    }
}

