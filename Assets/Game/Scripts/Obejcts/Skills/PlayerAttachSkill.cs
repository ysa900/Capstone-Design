using System.Collections;
using UnityEngine;

public class PlayerAttachSkill : Skill
{
    private float aliveTime; // 스킬 생존 시간을 체크할 변수
    SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTime > 0.5f || enemy == null;

        if (destroySkill)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            AttackPlayer();
        }

        aliveTime += Time.fixedDeltaTime;
    }

    // 플레이어에 붙어다니는 스킬
    private void AttackPlayer()
    {
        if (player.isPlayerLookLeft)
        {
            spriteRenderer.flipX = !player.isPlayerLookLeft; // 반대로 하는 이유는 기본 프리팹이 방향이 반대라서 (나중에 일관성있게 바꿀 필요 있음)
            X = player.gameObject.transform.position.x - 3f;
        }
        else
        {
            spriteRenderer.flipX = !player.isPlayerLookLeft;
            X = player.gameObject.transform.position.x + 3f;
        }

        Y = player.gameObject.transform.position.y + 0.2f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable == null)
        {
            return;
        }

        damageable.TakeDamage(gameObject, damage);

        StartCoroutine(Delay(0.5f));
        StopCoroutine(Delay(0.5f));
        
    }

    IEnumerator Delay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime); // 지정한 초 만큼 쉬기
        Destroy(gameObject);
    }
}

