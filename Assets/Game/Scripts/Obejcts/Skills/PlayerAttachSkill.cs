using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerAttachSkill : Skill, IPullingObject
{
    // 플레이어 좌표를 기준으로 위치를 어디로 가야하나를 받는 변수
    public float xPositionNum;
    public float yPositionNum;

    public bool isAttachSkill; // 플레이어 근처에 붙어다니는 스킬이냐
    public bool isFlipped; // 스킬 프리팹이 뒤집힌 상태냐
    public bool isCircleSkill; // 플레이어 주위를 빙빙 도는 스킬이냐
    public bool isShieldSkill; // Damage가 없는 Shield 스킬이냐
    public bool isDelaySkill; // 잠깐의 Delay 후 발사되는 스킬이냐
    public bool isYFlipped; // 스킬을 y축으로 뒤집어야되냐

    public float degree = 0f;
    private float tmpX; // Cirle을 계산할 때 0,0을 기준으로 생각한 X
    private float tmpY; // Cirle을 계산할 때 0,0을 기준으로 생각한 X

    private float delay = 0.45f;
    private float delayTimer = 0f;

    public float aliveTime; // 스킬 생존 시간을 체크할 변수
    private float aliveTimer; // 스킬 생존 시간
    SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수

    public delegate void OnShieldSkillDestroyed();
    public OnShieldSkillDestroyed onShieldSkillDestroyed;

    public new void Init()
    {
        aliveTimer = 0;
        DotDelayTimers.Clear();
        DotDamagedEnemies_Name.Clear();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            if (isShieldSkill)
                onShieldSkillDestroyed(); // 쉴드 스킬이 파괴될 땐 SkillManager에 알려준다

            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

            GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
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

        aliveTimer += Time.fixedDeltaTime;
        delayTimer += Time.fixedDeltaTime;

        if (isDotDamageSkill)
        {
            for(int i = 0; i < DotDelayTimers.Count; i++)
            {
                DotDelayTimers[i] += Time.fixedDeltaTime;
            }
        }
            
    }

    // 플레이어에 붙어다니는 스킬
    private void AttachPlayer()
    {
        if (player.isPlayerLookLeft)
        {
            X = player.transform.position.x - xPositionNum;
        }
        else
        {
            X = player.transform.position.x + xPositionNum;
        }

        Y = player.transform.position.y + yPositionNum;

        if (isDelaySkill)
        {
            if (isFlipped)
            {
                spriteRenderer.flipX = !player.isPlayerLookLeft;
            }
            else
            {
                spriteRenderer.flipX = player.isPlayerLookLeft;
            }
            
        }
        else
        {
            if (isFlipped)
            {
                spriteRenderer.flipX = !player.isPlayerLookLeft;
            }
            else if (isYFlipped)
            {
                spriteRenderer.flipY = !player.isPlayerLookLeft;
            }
            else
            {
                spriteRenderer.flipX = player.isPlayerLookLeft;
            }
        }
    }

    // 플레이어 주위를 빙빙 도는 스킬
    private void CircleMove()
    {
        degree -= speed;

        tmpX = (float)Math.Cos(degree * Mathf.Deg2Rad) * xPositionNum;
        tmpY = (float)Math.Sin(degree * Mathf.Deg2Rad) * xPositionNum; //이거 잘못쓴거 아님 (xPositionNum이 여기서 반지름 역할)

        X = tmpX + player.transform.position.x;
        Y = tmpY + player.transform.position.y;

        if (degree <= -360)
        {
            degree %= -360;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            if(isShieldSkill) { return; }

            if (isDelaySkill)
            {
                if (delayTimer >= delay)
                {
                    damageable.TakeDamage(gameObject, damage);

                    return;
                }
            }
            else
            {
                damageable.TakeDamage(gameObject, damage);

                return;
            }
        }

        IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();

        if (damageableSkill != null)
        {
            if (isShieldSkill) { return; }

            if (isDelaySkill)
            {
                if (delayTimer >= delay)
                {
                    damageable.TakeDamage(gameObject, damage);

                    return;
                }
            }
            else
            {
                damageable.TakeDamage(gameObject, damage);

                return;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDotDamageSkill)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();

            if (damageable != null)
            {
                int enemyIndex = DotDamagedEnemies_Name.FindIndex(x => x == collision.gameObject.name);

                if(enemyIndex == -1)
                {
                    DotDamagedEnemies_Name.Add(collision.gameObject.name);
                    DotDelayTimers.Add(1f);

                    enemyIndex = DotDamagedEnemies_Name.Count - 1;
                }

                if (DotDelayTimers[enemyIndex] < dotDelayTime) { return; }
                else
                {
                    if (isDelaySkill)
                    {
                        if (delayTimer >= delay)
                        {
                            damageable.TakeDamage(gameObject, damage);

                            DotDelayTimers[enemyIndex] = 0;
                        }
                    }
                    else
                    {
                        damageable.TakeDamage(gameObject, damage);

                        DotDelayTimers[enemyIndex] = 0;
                    }

                    return;
                }
            }

            IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();

            if (damageableSkill != null && isDotDamageSkill)
            {
                int enemyIndex = DotDamagedEnemies_Name.FindIndex(x => x == collision.gameObject.name);

                if (enemyIndex == -1)
                {
                    DotDamagedEnemies_Name.Add(collision.gameObject.name);
                    DotDelayTimers.Add(1f);

                    enemyIndex = DotDamagedEnemies_Name.Count - 1;
                }

                if (DotDelayTimers[enemyIndex] < dotDelayTime) { return; }
                else
                {
                    if (isDelaySkill)
                    {
                        if (delayTimer >= delay)
                        {
                            damageable.TakeDamage(gameObject, damage);

                            DotDelayTimers[enemyIndex] = 0;
                        }
                    }
                    else
                    {
                        damageable.TakeDamage(gameObject, damage);

                        DotDelayTimers[enemyIndex] = 0;
                    }

                    return;
                }
            }

            
        }
    }
}

